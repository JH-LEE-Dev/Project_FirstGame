using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsWorld;

public class ShopCardVisual : MonoBehaviour
{
    private ShopCardInstance owner;
    private RectTransform visual;

    private Vector2 basePos;

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


}
