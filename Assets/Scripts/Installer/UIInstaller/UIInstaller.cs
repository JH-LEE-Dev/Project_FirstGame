using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class UIInstaller : MonoBehaviour
{
    private InputManager inputManager;
    private UIManager uiManager;
    private ICardSystemProvider cardSystemProvider;
    private GameController gameController;
    private IGameFlowController gameFlowController;


    [Header("MainMenu Scene Objects")]
    [SerializeField] private CanvasRoot mainMenuLevelRoots_Prefab;

    [Header("Gameplay Scene Objects")]
    [SerializeField] private CanvasRoot gameplayLevelRoots_Prefab;
    [SerializeField] private Canvas canvas_GamplayScene_Prefab;
    [SerializeField] private Canvas canvas_MainMenuScene_Prefab;

    //MainMenu Scene
    private CanvasRoot mainMenuLevelRoots;
    private Canvas canvas_MainMenuScene;

    //Gameplay Scene
    private CanvasRoot gameplayLevelRoots;
    private Canvas canvas_GamplayScene;


    public void Initialize(IGameFlowController _gameFlowController,InputManager _inputManager)
    {
        inputManager = _inputManager;
        uiManager = GetComponent<UIManager>();
        gameFlowController = _gameFlowController;

        uiManager.Initialize(inputManager);
    }

    public void DependencyInjection_Gameplay(ICardSystemProvider _cardSystemProvider,GameController _gameController)
    {
        cardSystemProvider = _cardSystemProvider;
        gameController = _gameController;

        uiManager.Initialize_GameplayScene(_cardSystemProvider);

        SetupUIElement();

        GS_PlayerTurnState playerTurnState = gameController.GetGameState<GS_PlayerTurnState>();

        if (playerTurnState != null)
        {
            UIView_HUD uiViewHUD = uiManager.GetView<UIView_HUD>();
            UIView_CardSystem uIView_CardSystem = uiManager.GetView<UIView_CardSystem>();

            if (uiViewHUD != null)
            {
                playerTurnState.PlayerTurnStartEvent -= uiViewHUD.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent += uiViewHUD.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent -= uIView_CardSystem.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent += uIView_CardSystem.PlayerTurnStarted;
            }
        }
    }

    public void Release_Gameplay()
    {
        ReleaseDependency_GameplayScene();
        ReleaseEvent_Gameplay();
    }

    public void Release_MainMenu()
    {
        ReleaseEvent_MainMenu();
    }

    public void MainMenuLevelStarted()
    {
        canvas_MainMenuScene = Instantiate(canvas_MainMenuScene_Prefab);

        Transform overlayRoot = Instantiate(mainMenuLevelRoots_Prefab.overlayLayerRoot, canvas_MainMenuScene.transform);
        Transform popupLayerRoot = Instantiate(mainMenuLevelRoots_Prefab.popupLayerRoot, canvas_MainMenuScene.transform);
        //Transform screenLayerRoot = Instantiate(mainMenuLevelRoots_Prefab.screenLayerRoot, canvas_MainMenuScene.transform);
        //Transform tooltipLayerRoot = Instantiate(mainMenuLevelRoots_Prefab.tooltipLayerRoot, canvas_MainMenuScene.transform);

        SetAnchorToCanvas(overlayRoot);
        SetAnchorToCanvas(popupLayerRoot);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;

        uiManager.SceneChanged(tempRoot);
        uiManager.Initialize_MainMenuScene();

        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        mainMenuUIView.PlayButtonClickedEvent -= gameFlowController.GoToGameplayScene;
        mainMenuUIView.PlayButtonClickedEvent += gameFlowController.GoToGameplayScene;
    }

    public void GameplayLevelStarted()
    {

    }

    public void SetupUIElement()
    {
        canvas_GamplayScene = Instantiate(canvas_GamplayScene_Prefab);

        Transform overlayRoot = Instantiate(gameplayLevelRoots_Prefab.overlayLayerRoot, canvas_GamplayScene.transform);
        Transform popupLayerRoot = Instantiate(gameplayLevelRoots_Prefab.popupLayerRoot, canvas_GamplayScene.transform);
        //Transform screenLayerRoot = Instantiate(gameplayLevelRoots_Prefab.screenLayerRoot, canvas_GamplayScene.transform);
        //Transform tooltipLayerRoot = Instantiate(gameplayLevelRoots_Prefab.tooltipLayerRoot, canvas_GamplayScene.transform);

        SetAnchorToCanvas(overlayRoot);
        SetAnchorToCanvas(popupLayerRoot);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;
        uiManager.SceneChanged(tempRoot);

        OpenGameplayUIView();
    }

    public void OpenGameplayUIView()
    {
        UIView_HUD HUDObject = uiManager.Open<UIView_HUD>();
        UIView_CardSystem cardSystemObject = uiManager.Open<UIView_CardSystem>();
        UIView_Gameplay gameplayObject = uiManager.Open<UIView_Gameplay>();

        SetAnchorToCanvas(HUDObject.transform);
        SetAnchorToCanvas(cardSystemObject.transform);
        SetAnchorToCanvas(gameplayObject.transform);

        BindEvent_Gameplay(HUDObject, cardSystemObject, gameplayObject);
    }

    public void ResetVariable()
    {
        cardSystemProvider = null;
    }

    private void SetAnchorToCanvas(Transform transform)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;   // (0, 0)
        rt.anchorMax = Vector2.one;    // (1, 1)

        rt.offsetMin = Vector2.zero;   // Left, Bottom
        rt.offsetMax = Vector2.zero;   // Right, Top
    }

    private void BindEvent_Gameplay(UIView_HUD HUDObject, UIView_CardSystem cardSystemObject, UIView_Gameplay gameplayObject)
    {
        cardSystemProvider.CardPileDrawedEvent -= cardSystemObject.CardDrawed;
        cardSystemProvider.CardPileDrawedEvent += cardSystemObject.CardDrawed;
        cardSystemProvider.CardDrawFinishedEvent -= cardSystemObject.CardDrawFinished;
        cardSystemProvider.CardDrawFinishedEvent += cardSystemObject.CardDrawFinished;
        cardSystemObject.CardUsedEvent -= cardSystemProvider.CardUsed;
        cardSystemObject.CardUsedEvent += cardSystemProvider.CardUsed;
        cardSystemObject.TurnFinishedEvent -= cardSystemProvider.CardUsingFinished;
        cardSystemObject.TurnFinishedEvent += cardSystemProvider.CardUsingFinished;
        cardSystemProvider.CardDrawFinishedEvent -= HUDObject.CardUseTimeStarted;
        cardSystemProvider.CardDrawFinishedEvent += HUDObject.CardUseTimeStarted;
        cardSystemProvider.CardUsingFinishedEvent -= gameplayObject.CardUsingFinished;
        cardSystemProvider.CardUsingFinishedEvent += gameplayObject.CardUsingFinished;

        GS_EnemyTurnState enemyTurnState = gameController.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= HUDObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent += HUDObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= cardSystemObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent += cardSystemObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= gameplayObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent += gameplayObject.EnemyTurnStarted;
        }
    }

    private void ReleaseEvent_Gameplay()
    {
        UIView_HUD HUDObject = uiManager.GetView<UIView_HUD>();
        UIView_CardSystem cardSystemObject = uiManager.GetView<UIView_CardSystem>();
        UIView_Gameplay gameplayObject = uiManager.GetView<UIView_Gameplay>();

        cardSystemProvider.CardPileDrawedEvent -= cardSystemObject.CardDrawed;
        cardSystemProvider.CardDrawFinishedEvent -= cardSystemObject.CardDrawFinished;
        cardSystemObject.TurnFinishedEvent -= cardSystemProvider.CardUsingFinished;
        cardSystemObject.CardUsedEvent -= cardSystemProvider.CardUsed;
        cardSystemProvider.CardDrawFinishedEvent -= HUDObject.CardUseTimeStarted;
        cardSystemProvider.CardUsingFinishedEvent -= gameplayObject.CardUsingFinished;

        GS_EnemyTurnState enemyTurnState = gameController.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= HUDObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= cardSystemObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= gameplayObject.EnemyTurnStarted;
        }

        GS_PlayerTurnState playerTurnState = gameController.GetGameState<GS_PlayerTurnState>();

        if (playerTurnState != null)
        {
            UIView_HUD uiViewHUD = uiManager.GetView<UIView_HUD>();
            UIView_CardSystem uIView_CardSystem = uiManager.GetView<UIView_CardSystem>();

            if (uiViewHUD != null)
            {
                playerTurnState.PlayerTurnStartEvent -= uiViewHUD.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent -= uIView_CardSystem.PlayerTurnStarted;
            }
        }
    }

    public void ReleaseEvent_MainMenu()
    {
        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        mainMenuUIView.PlayButtonClickedEvent -= gameFlowController.GoToGameplayScene;
    }

    public void ReleaseDependency_GameplayScene()
    {
        uiManager.ReleaseDependency_GameplayScene();
    }
}
