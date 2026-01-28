using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class ShopUIInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private SignalHub signalHub;
    private IBootStrapProvider bootStrapProvider;
    private IShopSystemData shopSystemData;

    //내부 의존성
    private ShopUIManager uiManager;
    private ShopUICoordinator shopUICoordinator;
    private ShopUIModuleCoordinator shopUIModuleCoordinator;

    [Header("UI Canvas/CanvasRoot Objects")]
    [SerializeField] private CanvasRoot canvasRootPrefab;
    [SerializeField] private Canvas canvasPrefab;

    //Gameplay Scene
    private CanvasRoot canvasRoot;
    private Canvas canvas;

    public const string LAYER_SHOPUI = "ShopUI";

    public void Initialize(IBootStrapProvider _bootStrapProvider, InputManager _inputManager,SignalHub _signalHub,
        IShopSystemData _shopSystemData)
    {
        signalHub =_signalHub;
        bootStrapProvider = _bootStrapProvider;
        inputManager = _inputManager;
        uiManager = GetComponent<ShopUIManager>();
        shopUICoordinator = new ShopUICoordinator();
        shopUIModuleCoordinator = new ShopUIModuleCoordinator();    
        shopSystemData = _shopSystemData;

        uiManager.Initialize(inputManager,shopSystemData);

        SetupShopUI();
    }

    public void Release()
    {
        ReleaseEvent();

        shopUIModuleCoordinator.Release();
        shopUICoordinator.Release();
    }

    public void SetupShopUI()
    {
        SetupCanvas();

        Transform overlayRoot = Instantiate(canvasRootPrefab.overlayLayerRoot, canvas.transform);
        Transform popupLayerRoot = Instantiate(canvasRootPrefab.popupLayerRoot, canvas.transform);
        //Transform screenLayerRoot = Instantiate(canvasRootPrefab.screenLayerRoot, canvas.transform);
        //Transform tooltipLayerRoot = Instantiate(canvasRootPrefab.tooltipLayerRoot, canvas.transform);

        SetAnchorToCanvas(overlayRoot);
        SetAnchorToCanvas(popupLayerRoot);

        CanvasRoot tempRoot = new CanvasRoot();
        tempRoot.overlayLayerRoot = overlayRoot;
        tempRoot.popupLayerRoot = popupLayerRoot;
        uiManager.SceneChanged(tempRoot);

        SetupUIObjects();
        SetupCanvasChilds();
    }

    public void SetupCanvas()
    {
        canvas = Instantiate(canvasPrefab);
        CanvasEnabler canvasEnabler = canvas.GetComponent<CanvasEnabler>();

        if (canvasEnabler != null)
        {
            canvasEnabler.Initialize(LAYER_SHOPUI);
            StartCoroutine(canvasEnabler.InitializeChildrenCanvas());
        }
    }

    private void SetupCanvasChilds()
    {
        CanvasEnabler canvasEnabler = canvas.GetComponent<CanvasEnabler>();

        if (canvasEnabler != null)
        {
            canvasEnabler.Initialize();
        }
    }

    private void SetupUIObjects()
    {
        UIView_Shop shopUIView = uiManager.Open<UIView_Shop>();
        uiManager.Close<UIView_Shop>();

        shopUICoordinator.Initialize(shopUIView);
        shopUIModuleCoordinator.Initialize(signalHub,shopUICoordinator);

        BindEvent();
    }

    private void BindEvent()
    {
    }

    public void ReleaseEvent()
    {
    }

    private void SetAnchorToCanvas(Transform transform)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;   // (0, 0)
        rt.anchorMax = Vector2.one;    // (1, 1)

        rt.offsetMin = Vector2.zero;   // Left, Bottom
        rt.offsetMax = Vector2.zero;   // Right, Top
    }
}
