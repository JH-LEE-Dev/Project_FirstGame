using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class GameplayUIInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private IBootStrapProvider bootStrapProvider;
    private SignalHub signalHub;
    private ICardSystemData cardSystemData;
    private IWaveSystemData waveSystemData;
    private ICardLocalizationSystem cardLocalizationSystem;

    //내부 의존성
    private GameplayUIManager uiManager;
    private UICommandManager uiCommandManager;
    private GameplayUIModuleCoordinator gameplayUIModuleCoordinator;
    private CardUICoordinator cardUICoordinator;
    private GameplayUICoordinator gameplayUICoordinator;

    [Header("UI Canvas/CanvasRoot Objects")]
    [SerializeField] private CanvasRoot canvasRootPrefab;
    [SerializeField] private Canvas canvasPrefab;

    //Gameplay Scene
    private CanvasRoot canvasRoot;
    private Canvas canvas;

    public void Initialize(IBootStrapProvider _bootStrapProvider, SignalHub _signalHub,
        InputManager _inputManager, ICardSystemData _cardSystemData, IWaveSystemData _waveSystemData,
        ICardLocalizationSystem _cardLocalizationSystem)
    {
        inputManager = _inputManager;
        bootStrapProvider = _bootStrapProvider;
        signalHub = _signalHub;
        cardSystemData = _cardSystemData;
        waveSystemData = _waveSystemData;
        cardLocalizationSystem = _cardLocalizationSystem; 

        uiManager = GetComponent<GameplayUIManager>();
        uiCommandManager = GetComponent<UICommandManager>();
        gameplayUIModuleCoordinator = new GameplayUIModuleCoordinator();
        cardUICoordinator = new CardUICoordinator();
        gameplayUICoordinator = new GameplayUICoordinator();

        uiCommandManager.Initialize(signalHub);
        uiManager.Initialize(inputManager, cardSystemData, waveSystemData,cardLocalizationSystem);

        SetupUIElement();
    }

    public void Release()
    {
        gameplayUIModuleCoordinator.Release();
        cardUICoordinator.Release();
        gameplayUICoordinator.Release();
        uiCommandManager.Release();

        ReleaseDependency();
        ReleaseEvent();
    }

    public void SetupUIElement()
    {
        SetupCanvas();

        Transform overlayRoot = Instantiate(canvasRootPrefab.overlayLayerRoot, canvas.transform);
        Transform popupLayerRoot = Instantiate(canvasRootPrefab.popupLayerRoot, canvas.transform);
        Transform worldLayerRoot = Instantiate(canvasRootPrefab.worldLayerRoot, null);
        //Transform screenLayerRoot = Instantiate(gameplayLevelRoots_Prefab.screenLayerRoot, canvas_GamplayScene.transform);
        //Transform tooltipLayerRoot = Instantiate(gameplayLevelRoots_Prefab.tooltipLayerRoot, canvas_GamplayScene.transform);

        SetAnchorToCanvas(overlayRoot);
        SetAnchorToCanvas(popupLayerRoot);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;
        tempRoot.worldLayerRoot = worldLayerRoot;
        uiManager.SceneChanged(tempRoot);

        OpenUIView();
        SetupCanvasChilds();
    }

    public void SetupCanvas()
    {
        canvas = Instantiate(canvasPrefab);
        CanvasEnabler canvasEnabler = canvas.GetComponent<CanvasEnabler>();

        if (canvasEnabler != null)
        {
            canvasEnabler.Initialize();
        }
    }

    private void SetupCanvasChilds()
    {
        CanvasEnabler canvasEnabler = canvas.GetComponent<CanvasEnabler>();

        if (canvasEnabler != null)
        {
            canvasEnabler.Initialize();
            StartCoroutine(canvasEnabler.InitializeChildrenCanvas());
        }
    }

    private void OpenUIView()
    {
        UIView_HUD hudObject = uiManager.Open<UIView_HUD>();
        UIView_CardSystem cardSystemObject = uiManager.Open<UIView_CardSystem>();
        UIView_Gameplay gameplayObject = uiManager.Open<UIView_Gameplay>();
        UIView_Unit_World unitWorldUIObject = uiManager.Open<UIView_Unit_World>();
        UIView_Unit_Canvas unitCanvasUIObject = uiManager.Open<UIView_Unit_Canvas>();

        cardUICoordinator.Initialize(cardSystemObject);
        gameplayUICoordinator.Initialize(hudObject, unitWorldUIObject, gameplayObject, unitCanvasUIObject);
        gameplayUIModuleCoordinator.Initialize(signalHub, cardUICoordinator, gameplayUICoordinator);

        SetAnchorToCanvas(hudObject.transform);
        SetAnchorToCanvas(cardSystemObject.transform);
        SetAnchorToCanvas(gameplayObject.transform);

        BindEvent();
    }

    private void SetAnchorToCanvas(Transform transform)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;   // (0, 0)
        rt.anchorMax = Vector2.one;    // (1, 1)

        rt.offsetMin = Vector2.zero;   // Left, Bottom
        rt.offsetMax = Vector2.zero;   // Right, Top
    }

    public void SetupUI()
    {
        BindEvent();
    }

    private void BindEvent()
    {
       
    }

    private void ReleaseEvent()
    {

    }

    public void ReleaseDependency()
    {
        uiManager.ReleaseDependency();
    }
}
