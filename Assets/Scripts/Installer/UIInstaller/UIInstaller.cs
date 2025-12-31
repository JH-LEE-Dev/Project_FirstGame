using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UIInstaller : MonoBehaviour
{
    private UIManager uiManager;

    private IDeckProvider deckProvider;

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


    public void Initialize(IGameFlowController _gameFlowController)
    {
        uiManager = GetComponent<UIManager>();
        gameFlowController = _gameFlowController;
    }

    public void DependencyInjection_Gameplay(IDeckProvider _deckProvider)
    {
        deckProvider = _deckProvider;
    }

    public void Release()
    {

    }

    public void MainMenuLevelStarted()
    {
        canvas_MainMenuScene = Instantiate(canvas_MainMenuScene_Prefab);

        Transform overlayRoot = Instantiate(mainMenuLevelRoots_Prefab.overlayLayerRoot, canvas_MainMenuScene.transform);
        Transform popupLayerRoot = Instantiate(mainMenuLevelRoots_Prefab.popupLayerRoot, canvas_MainMenuScene.transform);
        //Transform screenLayerRoot = Instantiate(mainMenuLevelRoots_Prefab.screenLayerRoot, canvas_MainMenuScene.transform);
        //Transform tooltipLayerRoot = Instantiate(mainMenuLevelRoots_Prefab.tooltipLayerRoot, canvas_MainMenuScene.transform);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;

        uiManager.SceneChanged(tempRoot);
        uiManager.Initialize_MainMenuScene();

        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();
        mainMenuUIView.PlayButtonClickedEvent += gameFlowController.GoToGameplayScene;
    }

    public void GameplayLevelStarted()
    {
        ResetVariable();

        canvas_GamplayScene = Instantiate(canvas_GamplayScene_Prefab);

        Transform overlayRoot = Instantiate(gameplayLevelRoots_Prefab.overlayLayerRoot, canvas_GamplayScene.transform);
        Transform popupLayerRoot = Instantiate(gameplayLevelRoots_Prefab.popupLayerRoot, canvas_GamplayScene.transform);
        //Transform screenLayerRoot = Instantiate(gameplayLevelRoots_Prefab.screenLayerRoot, canvas_GamplayScene.transform);
        //Transform tooltipLayerRoot = Instantiate(gameplayLevelRoots_Prefab.tooltipLayerRoot, canvas_GamplayScene.transform);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;
        uiManager.SceneChanged(tempRoot);
        uiManager.Initialize_GameplayScene(deckProvider);

        UIView_HUD HUDObject = uiManager.Open<UIView_HUD>();
        UIView_CardSystem cardSystem = uiManager.Open<UIView_CardSystem>();
    }

    public void ResetVariable()
    {
        deckProvider = null;
        uiManager.ResetVariable();
    }
}
