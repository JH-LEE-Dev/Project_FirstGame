using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    protected UIViewContext viewCtx;

    [Header("UIView Settings")]
    [SerializeField] private UILayer layer = UILayer.Screen;
    [SerializeField] private bool startHidden = true;

    public UILayer Layer => layer;

    private bool _isVisible;

    protected virtual void Awake()
    {
        if (startHidden)
        {
            gameObject.SetActive(false);
            _isVisible = false;
        }
        else
        {
            _isVisible = gameObject.activeSelf;
        }
    }

    public virtual void Update()
    {
        
    }

    public void Initialize(UIViewContext ctx)
    {
        viewCtx = ctx;

        SetupUI();
    }

    public virtual void SetupUI()
    {

    }

    /// <summary>
    /// 외부에서 View를 열 때 호출
    /// </summary>
    public virtual void Show()
    {
        if (_isVisible)
            return;

        _isVisible = true;
        gameObject.SetActive(true);
        OnShow();
    }

    /// <summary>
    /// 외부에서 View를 닫을 때 호출
    /// </summary>
    public virtual void Hide()
    {
        if (!_isVisible)
            return;

        _isVisible = false;
        OnHide();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show() 직후 호출되는 훅
    /// </summary>
    protected virtual void OnShow() { }

    /// <summary>
    /// Hide() 직전 호출되는 훅
    /// </summary>
    protected virtual void OnHide() { }

    protected virtual void SetAnchorToCanvas(Transform transform)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;   // (0, 0)
        rt.anchorMax = Vector2.one;    // (1, 1)

        rt.offsetMin = Vector2.zero;   // Left, Bottom
        rt.offsetMax = Vector2.zero;   // Right, Top
    }
}