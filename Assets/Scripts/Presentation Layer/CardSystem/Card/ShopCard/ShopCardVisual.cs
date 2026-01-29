using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsWorld;

public class ShopCardVisual : MonoBehaviour
{
    private ShopCardInstance owner;
    private RectTransform visual;

    private Vector2 basePos;
    private float seed;

    [Header("Float")]
    [SerializeField] private float floatPosAmp = 0.5f;
    [SerializeField] private float floatRotAmp = 0.2f;
    [SerializeField] private float FloatFreq = 0.2f;


    [Header("Overlay Ref")]
    [SerializeField] private Image drawOverlay;

    [Header("CanvasGroup")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Draw Look")]
    private Color drawColor = new Color(1f, 1f, 0.8f, 1f);

    private void Awake()
    {
        if (drawOverlay != null)
        {
            var c = drawColor;
            c.a = 0f;
            drawOverlay.color = c;
            drawOverlay.raycastTarget = false;
        }

        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Bind(ShopCardInstance card)
    {
        owner = card;
        visual = GetComponent<RectTransform>();
        basePos = visual.anchoredPosition;
        seed = UnityEngine.Random.Range(0f, 1000f);
    }

    private void Update()
    {
        Floating();
    }

    public void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
    private void Floating()
    {
        float t = Time.unscaledTime + seed;
        float w = FloatFreq * Mathf.PI * 2f;

        float x = Mathf.Sin(t * w) * floatPosAmp;
        float y = Mathf.Cos(t * w * 1.13f) * (floatPosAmp * 0.8f);
        float rz = Mathf.Sin(t * w * 0.9f) * floatRotAmp;

        visual.anchoredPosition = basePos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }

}
