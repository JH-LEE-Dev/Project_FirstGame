using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class CardVisualFloat : MonoBehaviour
{
    private CardInstance owner;
    private RectTransform visual;

    private Vector2 basePos;
    private float seed;

    [Header("Hand Float")]
    [SerializeField] private float handFloatPosAmp = 0.5f;
    [SerializeField] private float handFloatRotAmp = 0.2f;
    [SerializeField] private float handFloatFreq = 0.2f;

    [Header("Preview Float")]
    [SerializeField] private float previewFloatPosAmp = 0.5f;
    [SerializeField] private float previewFloatRotAmp = 0.15f;
    [SerializeField] private float previewFloatFreq = 0.2f;

    [Header("Draw Look")]
    [SerializeField] private Color drawColor = new Color(1f, 1f, 0.07f, 1f);
    [SerializeField] private float colorDuration = 0.9f;

    [Header("Overlay Ref")]
    [SerializeField] private Image drawOverlay;
    private Tween drawTween;


    private void Awake()
    {
        if (drawOverlay != null)
        {
            var c = drawColor;
            c.a = 0f;
            drawOverlay.color = c;
            drawOverlay.raycastTarget = false;
        }
    }

    public void Bind(CardInstance card)
    {
        owner = card;
        visual = GetComponent<RectTransform>();
        basePos = visual.anchoredPosition;
        seed = UnityEngine.Random.Range(0f, 1000f);
    }
    
    private void Update()
    {
        if (owner == null || owner.Motion == null) return;

        if (owner.Motion.IgnoreHandLayout) PreviewFloating();
        else HandFloating();
    }

    private void HandFloating()
    {
        float t = Time.unscaledTime + seed;
        float w = handFloatFreq * Mathf.PI * 2f;

        float x = Mathf.Sin(t * w) * handFloatPosAmp;
        float y = Mathf.Cos(t * w * 1.13f) * (handFloatPosAmp * 0.8f);
        float rz = Mathf.Sin(t * w * 0.9f) * handFloatRotAmp;

        visual.anchoredPosition = basePos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }

    private void PreviewFloating()
    {
        float t = Time.unscaledTime + seed;
        float w = previewFloatFreq * Mathf.PI * 2f;

        float x = Mathf.Sin(t * w) * previewFloatPosAmp;
        float y = Mathf.Cos(t * w * 1.07f) * (previewFloatPosAmp * 0.8f);
        float rz = Mathf.Sin(t * w * 0.85f) * previewFloatRotAmp;

        visual.anchoredPosition = basePos + new Vector2(x, y);
        visual.localRotation = Quaternion.Euler(0f, 0f, rz);
    }

    public void PlayDrawColor()
    {
        if (drawOverlay == null) return;

        drawTween?.Kill();

        var c = drawColor;
        c.a = 1f;
        drawOverlay.color = c;

        drawTween = drawOverlay.DOFade(0f, Mathf.Max(0.01f, colorDuration))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}
