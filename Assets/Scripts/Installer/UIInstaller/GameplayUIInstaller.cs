using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class GameplayUIInstaller : MonoBehaviour,IUIModuleProvider
{
    public ICardUICommandSystem uiCommandSystem => uiCommandManager;

    //외부 의존성
    private InputManager inputManager;
    private IBootStrapProvider bootStrapProvider;
    private SignalHub signalHub;
    private ICardSystemData cardSystemData;
    private IWaveSystemData waveSystemData;

    //내부 의존성
    private GameplayUIManager uiManager;
    private UICommandManager uiCommandManager;
    private CardUICoordinator cardUICoordinator;
    private GameplayUICoordinator gameplayUICoordinator;

    [Header("Gameplay Scene Objects")]
    [SerializeField] private CanvasRoot canvasRootPrefab;
    [SerializeField] private Canvas canvasPrefab;

    //Gameplay Scene
    private CanvasRoot canvasRoot;
    private Canvas canvas;


    public void Initialize(IBootStrapProvider _bootStrapProvider,SignalHub _signalHub, 
        InputManager _inputManager,ICardSystemData _cardSystemData,IWaveSystemData _waveSystemData)
    {
        inputManager = _inputManager;
        bootStrapProvider = _bootStrapProvider;
        signalHub = _signalHub;
        cardSystemData = _cardSystemData;
        waveSystemData = _waveSystemData;

        uiManager = GetComponent<GameplayUIManager>();
        uiCommandManager = GetComponent<UICommandManager>();
        cardUICoordinator = new CardUICoordinator();
        gameplayUICoordinator = new GameplayUICoordinator();

        uiCommandManager.Initialize(signalHub);
        uiManager.Initialize(inputManager,cardSystemData,waveSystemData);

        SetupUIElement();
    }

    public void Release()
    {
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
        //Transform screenLayerRoot = Instantiate(gameplayLevelRoots_Prefab.screenLayerRoot, canvas_GamplayScene.transform);
        //Transform tooltipLayerRoot = Instantiate(gameplayLevelRoots_Prefab.tooltipLayerRoot, canvas_GamplayScene.transform);

        SetAnchorToCanvas(overlayRoot);
        SetAnchorToCanvas(popupLayerRoot);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;
        uiManager.SceneChanged(tempRoot);

        OpenUIView();
        SetupCanvasChilds();
    }

    public void SetupCanvas()
    {
        canvas = Instantiate(canvasPrefab);
        CanvasSystem canvasSystem = canvas.GetComponent<CanvasSystem>();

        if (canvasSystem != null)
        {
            canvasSystem.Initialize();
        }
    }

    private void SetupCanvasChilds()
    {
        CanvasSystem canvasSystem = canvas.GetComponent<CanvasSystem>();

        if (canvasSystem != null)
        {
            canvasSystem.Initialize();
            StartCoroutine(canvasSystem.InitializeChildrenCanvas());
        }
    }

    private void OpenUIView()
    {
        UIView_HUD hudObject = uiManager.Open<UIView_HUD>();
        UIView_CardSystem cardSystemObject = uiManager.Open<UIView_CardSystem>();
        UIView_Gameplay gameplayObject = uiManager.Open<UIView_Gameplay>();
        UIView_Unit unitUIObject = uiManager.Open<UIView_Unit>();

        cardUICoordinator.Initialize(signalHub,cardSystemObject, unitUIObject);
        gameplayUICoordinator.Initialize(signalHub,hudObject, unitUIObject, gameplayObject);

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
