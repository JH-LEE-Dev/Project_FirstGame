using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private UIViewContext viewCtx;

    private Transform screenLayerRoot;
    private Transform popupLayerRoot;
    private Transform overlayLayerRoot;
    private Transform tooltipLayerRoot;

    [Header("등록된 UIView Prefab들")]
    [SerializeField] private List<UIView> viewPrefabs = new List<UIView>();

    // 타입별 Prefab 캐시
    private Dictionary<Type, UIView> prefabByType = new Dictionary<Type, UIView>();

    // 타입별 인스턴스 캐시
    private Dictionary<Type, UIView> instanceByType = new Dictionary<Type, UIView>();

    public void SceneChanged(CanvasRoot canvasRoot)
    {
        CloseAll();

        screenLayerRoot = canvasRoot.screenLayerRoot;
        popupLayerRoot = canvasRoot.popupLayerRoot;
        overlayLayerRoot = canvasRoot.overlayLayerRoot;
        tooltipLayerRoot = canvasRoot.tooltipLayerRoot;
    }

    public void Initialize(InputManager inputManager)
    {
        viewCtx.Initialize(inputManager);
    }

    private void Awake()
    {
        viewCtx = new UIViewContext();

        // 타입별 Prefab 사전 구성
        foreach (var view in viewPrefabs)
        {
            if (view == null)
                continue;

            var type = view.GetType();

            if (!prefabByType.ContainsKey(type))
            {
                prefabByType.Add(type, view);
            }
            else
            {
                Debug.LogWarning($"[UIManager] 동일 타입 UIView Prefab이 중복 등록됨: {type.Name}");
            }
        }
    }

    /// <summary>
    /// T 타입의 UIView를 연다. 없으면 Instantiate해서 생성.
    /// </summary>
    public T Open<T>() where T : UIView
    {
        var type = typeof(T);

        if (!instanceByType.TryGetValue(type, out UIView instance) || instance == null)
        {
            instance = CreateViewInstance<T>();
            instanceByType[type] = instance;
        }

        instance.Show();

        return (T)instance;
    }

    /// <summary>
    /// T 타입의 UIView를 닫는다.
    /// </summary>
    public void Close<T>() where T : UIView
    {
        var type = typeof(T);

        if (instanceByType.TryGetValue(type, out UIView instance) && instance != null)
        {
            instance.Hide();
        }
    }

    /// <summary>
    /// 현재 생성되어 있는 모든 UIView를 닫는다.
    /// </summary>
    public void CloseAll()
    {
        ResetVariable();

        foreach (var kv in instanceByType)
        {
            UIView view = kv.Value;
            if (view != null)
            {
                view.Hide();
            }
        }
    }

    /// <summary>
    /// 특정 타입의 UIView 인스턴스를 반환(있을 때만)
    /// </summary>
    public T GetView<T>() where T : UIView
    {
        var type = typeof(T);
        if (instanceByType.TryGetValue(type, out UIView instance))
        {
            return instance as T;
        }
        return null;
    }

    private T CreateViewInstance<T>() where T : UIView
    {
        var type = typeof(T);

        if (!prefabByType.TryGetValue(type, out UIView prefab) || prefab == null)
        {
            Debug.LogError($"[UIManager] {type.Name} 타입의 UIView Prefab이 등록되어 있지 않습니다.");
            return null;
        }

        Transform parent = GetLayerRoot(prefab.Layer);

        if(parent == null)
        {
            Debug.Log("NULL!");
        }

        UIView instance = Instantiate(prefab, parent);
        instance.gameObject.name = $"{prefab.gameObject.name}_Instance";

        instance.Initialize(viewCtx);

        return (T)instance;
    }

    private Transform GetLayerRoot(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Screen: return screenLayerRoot;
            case UILayer.Popup: return popupLayerRoot;
            case UILayer.Overlay: return overlayLayerRoot;
            case UILayer.Tooltip: return tooltipLayerRoot;
            default: return screenLayerRoot;
        }
    }

    public void Initialize_GameplayScene(ICardSystemProvider cardSystemProvider)
    {
        viewCtx.Initialize_Gameplay(cardSystemProvider);
    }
    public void Initialize_MainMenuScene()
    {
    }

    public void ResetVariable()
    {
        viewCtx.ResetVariable();
    }
}