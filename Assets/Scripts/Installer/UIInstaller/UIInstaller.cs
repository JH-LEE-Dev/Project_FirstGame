using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class UIInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private ICardSystemActions cardSystemActions;
    private ICardSystemEvents cardSystemEvent;
    private IGameFlowProvider gameFlowProvider;
    private IBootStrapProvider bootStrapProvider;
    private IUnitLogicSystemData unitLogicSystemData;
    private IUnitEventAccessor unitEventAccessor;
    private IUnitSpawnSystemEvent unitSpawnSystemEvent;
    private ICardSystemData cardSystemData;

    //내부 의존성
    private UIManager uiManager;
    private UICommandManager uiCommandManager;
    private CardUICoordinator cardUICoordinator;


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


    public void Initialize(IBootStrapProvider _bootStrapProvider, InputManager _inputManager)
    {
        inputManager = _inputManager;
        bootStrapProvider = _bootStrapProvider;
        uiManager = GetComponent<UIManager>();
        uiCommandManager = GetComponent<UICommandManager>();
        cardUICoordinator = new CardUICoordinator();

        uiCommandManager.Initialize();
        uiManager.Initialize(inputManager);

    }

    public void ReceiveDependency_Gameplay(ICardSystemEvents _cardSystemEvent,ICardSystemData _cardSystemData,
        ICardSystemActions _cardSystemActions, IGameFlowProvider _gameFlowProvider,
        IUnitLogicSystemData _unitLogicSystemData, IUnitEventAccessor _unitEventAccessor,
        IUnitSpawnSystemEvent _unitSpawnSystemEvent)
    {
        cardSystemEvent = _cardSystemEvent;
        cardSystemActions = _cardSystemActions;
        gameFlowProvider = _gameFlowProvider;
        unitLogicSystemData = _unitLogicSystemData;
        unitEventAccessor = _unitEventAccessor;
        unitSpawnSystemEvent = _unitSpawnSystemEvent;
        cardSystemData = _cardSystemData;

        uiManager.Initialize_GameplayScene(cardSystemData,unitLogicSystemData);
        SetupUIElement();

        GS_PlayerTurnState playerTurnState = gameFlowProvider.GetGameState<GS_PlayerTurnState>();

        if (playerTurnState != null)
        {
            UIView_HUD uiViewHUD = uiManager.GetView<UIView_HUD>();

            if (uiViewHUD != null)
            {
                playerTurnState.PlayerTurnStartEvent -= uiViewHUD.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent += uiViewHUD.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent -= cardUICoordinator.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent += cardUICoordinator.PlayerTurnStarted;
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
        SetupMainMenuCanvas();

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

        OpenMainMenuUIView();
        SetupMainMenuCanvasChilds();
    }

    public void GameplayLevelStarted()
    {

    }

    public void SetupUIElement()
    {
        SetupGameplayCanvas();

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
        SetupGameplayCanvasChilds();
    }

    public void SetupGameplayCanvas()
    {
        canvas_GamplayScene = Instantiate(canvas_GamplayScene_Prefab);
        CanvasSystem canvasSystem = canvas_GamplayScene.GetComponent<CanvasSystem>();

        if (canvasSystem != null)
        {
            canvasSystem.Initialize();
        }
    }

    public void SetupMainMenuCanvas()
    {
        canvas_MainMenuScene = Instantiate(canvas_MainMenuScene_Prefab);
        CanvasSystem canvasSystem = canvas_MainMenuScene.GetComponent<CanvasSystem>();

        if (canvasSystem != null)
        {
            canvasSystem.Initialize();
            StartCoroutine(canvasSystem.InitializeChildrenCanvas());
        }
    }

    private void SetupGameplayCanvasChilds()
    {
        CanvasSystem canvasSystem = canvas_GamplayScene.GetComponent<CanvasSystem>();

        if (canvasSystem != null)
        {
            canvasSystem.Initialize();
            StartCoroutine(canvasSystem.InitializeChildrenCanvas());
        }
    }

    private void SetupMainMenuCanvasChilds()
    {
        CanvasSystem canvasSystem = canvas_MainMenuScene.GetComponent<CanvasSystem>();

        if (canvasSystem != null)
        {
            canvasSystem.Initialize();
        }
    }

    private void OpenGameplayUIView()
    {
        UIView_HUD HUDObject = uiManager.Open<UIView_HUD>();
        UIView_CardSystem cardSystemObject = uiManager.Open<UIView_CardSystem>();
        UIView_Gameplay gameplayObject = uiManager.Open<UIView_Gameplay>();
        UIView_Unit unitUIObject = uiManager.Open<UIView_Unit>();

        cardUICoordinator.Initialize(cardSystemObject, unitUIObject);

        SetAnchorToCanvas(HUDObject.transform);
        SetAnchorToCanvas(cardSystemObject.transform);
        SetAnchorToCanvas(gameplayObject.transform);

        BindEvent_Gameplay(HUDObject, gameplayObject);
    }

    private void OpenMainMenuUIView()
    {
        uiManager.Initialize_MainMenuScene();

        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        mainMenuUIView.PlayButtonClickedEvent -= bootStrapProvider.GoToGameplayScene;
        mainMenuUIView.PlayButtonClickedEvent += bootStrapProvider.GoToGameplayScene;
    }

    private void SetAnchorToCanvas(Transform transform)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;   // (0, 0)
        rt.anchorMax = Vector2.one;    // (1, 1)

        rt.offsetMin = Vector2.zero;   // Left, Bottom
        rt.offsetMax = Vector2.zero;   // Right, Top
    }

    private void BindEvent_Gameplay(UIView_HUD HUDObject, UIView_Gameplay gameplayObject)
    {
        cardSystemEvent.CardDrawFinishedEvent -= cardUICoordinator.CardDrawFinished;
        cardSystemEvent.CardDrawFinishedEvent += cardUICoordinator.CardDrawFinished;
        cardSystemEvent.CardUsingVerificationEvent -= cardUICoordinator.CardUsingApproved;
        cardSystemEvent.CardUsingVerificationEvent += cardUICoordinator.CardUsingApproved;
        cardSystemEvent.CardDrawFinishedEvent -= HUDObject.CardUseTimeStarted;
        cardSystemEvent.CardDrawFinishedEvent += HUDObject.CardUseTimeStarted;
        unitSpawnSystemEvent.PlayerSpawnedEvent -= cardUICoordinator.PlayerSpawned;
        unitSpawnSystemEvent.PlayerSpawnedEvent += cardUICoordinator.PlayerSpawned;
        unitSpawnSystemEvent.PlayerSpawnedEvent -= HUDObject.PlayerSpawned;
        unitSpawnSystemEvent.PlayerSpawnedEvent += HUDObject.PlayerSpawned;
        cardSystemEvent.CardUsingTurnFinishedEvent -= gameplayObject.CardUsingFinished;
        cardSystemEvent.CardUsingTurnFinishedEvent += gameplayObject.CardUsingFinished;

        uiCommandManager.JobDispatchEvent -= cardUICoordinator.RecieveUIJob;
        uiCommandManager.JobDispatchEvent += cardUICoordinator.RecieveUIJob;

        cardUICoordinator.UICommandCompleteEvent -= uiCommandManager.DecreaseJobBatchCount;
        cardUICoordinator.UICommandCompleteEvent += uiCommandManager.DecreaseJobBatchCount;
        cardUICoordinator.CardUsedEvent -= cardSystemActions.CardUsed;
        cardUICoordinator.CardUsedEvent += cardSystemActions.CardUsed;
        cardUICoordinator.CardUsingFinishedEvent -= cardSystemActions.CardUsingFinished;
        cardUICoordinator.CardUsingFinishedEvent += cardSystemActions.CardUsingFinished;

        IUnitEvent playerEventSource = unitEventAccessor.GetPlayerEventSource();
        playerEventSource.TakeDamageEvent -= HUDObject.OnPlayerHit;
        playerEventSource.TakeDamageEvent += HUDObject.OnPlayerHit;

        GS_EnemyTurnState enemyTurnState = gameFlowProvider.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= HUDObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent += HUDObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= cardUICoordinator.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent += cardUICoordinator.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= gameplayObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent += gameplayObject.EnemyTurnStarted;
        }
    }

    private void ReleaseEvent_Gameplay()
    {
        UIView_HUD HUDObject = uiManager.GetView<UIView_HUD>();
        UIView_Gameplay gameplayObject = uiManager.GetView<UIView_Gameplay>();

        cardSystemEvent.CardDrawFinishedEvent -= cardUICoordinator.CardDrawFinished;
        cardSystemEvent.CardUsingVerificationEvent -= cardUICoordinator.CardUsingApproved;
        cardSystemEvent.CardDrawFinishedEvent -= HUDObject.CardUseTimeStarted;
        unitSpawnSystemEvent.PlayerSpawnedEvent -= HUDObject.PlayerSpawned;
        unitSpawnSystemEvent.PlayerSpawnedEvent -= cardUICoordinator.PlayerSpawned;
        cardSystemEvent.CardUsingTurnFinishedEvent -= gameplayObject.CardUsingFinished;

        uiCommandManager.JobDispatchEvent -= cardUICoordinator.RecieveUIJob;
        cardUICoordinator.UICommandCompleteEvent -= uiCommandManager.DecreaseJobBatchCount;
        cardUICoordinator.CardUsedEvent -= cardSystemActions.CardUsed;
        cardUICoordinator.CardUsingFinishedEvent -= cardSystemActions.CardUsingFinished;

        GS_EnemyTurnState enemyTurnState = gameFlowProvider.GetGameState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= HUDObject.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= cardUICoordinator.EnemyTurnStarted;
            enemyTurnState.EnemyTurnStartEvent -= gameplayObject.EnemyTurnStarted;
        }

        GS_PlayerTurnState playerTurnState = gameFlowProvider.GetGameState<GS_PlayerTurnState>();

        if (playerTurnState != null)
        {
            if (HUDObject != null)
            {
                playerTurnState.PlayerTurnStartEvent -= HUDObject.PlayerTurnStarted;
                playerTurnState.PlayerTurnStartEvent -= cardUICoordinator.PlayerTurnStarted;
            }
        }

        cardUICoordinator.Release();
    }

    public void ReleaseEvent_MainMenu()
    {
        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        mainMenuUIView.PlayButtonClickedEvent -= bootStrapProvider.GoToGameplayScene;
    }

    public void ReleaseDependency_GameplayScene()
    {
        uiManager.ReleaseDependency_GameplayScene();
    }

    public void DependencyInjection_Gameplay(GameInstaller gameInstaller)
    {
        gameInstaller.ReceiveDependency_Gameplay(uiCommandManager);
    }
}
