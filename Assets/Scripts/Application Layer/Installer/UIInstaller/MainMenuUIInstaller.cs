using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class MainMenuUIInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private IBootStrapProvider bootStrapProvider;

    //내부 의존성
    private MainMenuUIManager uiManager;

    [Header("UI Canvas/CanvasRoot Objects")]
    [SerializeField] private CanvasRoot canvasRootPrefab;
    [SerializeField] private Canvas canvasPrefab;

    //Gameplay Scene
    private CanvasRoot canvasRoot;
    private Canvas canvas;

    public void Initialize(IBootStrapProvider _bootStrapProvider, InputManager _inputManager)
    {
        bootStrapProvider = _bootStrapProvider;
        inputManager = _inputManager;
        uiManager = GetComponent<MainMenuUIManager>();

        uiManager.Initialize(inputManager,null);
    }

    public void Release()
    {
        ReleaseEvent();
    }

    public void MainMenuLevelStarted()
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

    private void OpenUIView()
    {
        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        BindEvent();
    }

    private void BindEvent()
    {
        UIView_MainMenu mainMenuUIView = uiManager.GetView<UIView_MainMenu>();

        if (mainMenuUIView != null)
        {
            mainMenuUIView.PlayButtonClickedEvent -= bootStrapProvider.GoToGameplayScene;
            mainMenuUIView.PlayButtonClickedEvent += bootStrapProvider.GoToGameplayScene;
        }
    }

    private void SetAnchorToCanvas(Transform transform)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;   // (0, 0)
        rt.anchorMax = Vector2.one;    // (1, 1)

        rt.offsetMin = Vector2.zero;   // Left, Bottom
        rt.offsetMax = Vector2.zero;   // Right, Top
    }

    public void ReleaseEvent()
    {
        UIView_MainMenu mainMenuUIView = uiManager.Open<UIView_MainMenu>();

        mainMenuUIView.PlayButtonClickedEvent -= bootStrapProvider.GoToGameplayScene;
    }
}
