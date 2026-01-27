using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class CardVisualFloat : MonoBehaviour
{
    private MainCardInstance owner;
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
    private Color drawColor = new Color(1f, 1f, 0.5f, 1f);
    private float colorDuration = 0.5f;

    [Header("Overlay Ref")]
    [SerializeField] private Image drawOverlay;
    private Tween drawTween;

    [Header("Draw Pop")]
    [SerializeField] private float drawStartScale = 0.1f;
    [SerializeField] private float drawOvershootScale = 1.05f;
    [SerializeField] private float drawTotalDuration = 0.4f;
    private Tween drawScaleTween;



    [Header("CanvasGroup")]
    [SerializeField] private CanvasGroup canvasGroup;

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

    public void Bind(MainCardInstance card)
    {
        owner = card;
        visual = GetComponent<RectTransform>();
        basePos = visual.anchoredPosition;
        seed = UnityEngine.Random.Range(0f, 1000f);
    }
    
    private void Update()
    {
        if (owner == null || owner.Motion == null) return;

        if (owner.cardState == CardState.Preview || owner.cardState == CardState.Selecting) 
            PreviewFloating();
        else if (owner.cardState == CardState.InHand)
            HandFloating();
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


        drawScaleTween?.Kill();

        visual.localScale = Vector3.one * drawStartScale;

        float upDuration = drawTotalDuration * 0.8f;
        float downDuration = drawTotalDuration * 0.2f;

        drawScaleTween = DOTween.Sequence()
            .Append(
                visual.DOScale(drawOvershootScale, upDuration)
                    .SetEase(Ease.OutBack)
            )
            .Append(
                visual.DOScale(1f, downDuration)
                    .SetEase(Ease.OutCubic)
            )
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        SetVisible(true);
    }


    public void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }

    public Tween FadeDrawOverlayAlpha(float to, float dur)
    {
        if (drawOverlay == null) return null;

        drawTween?.Kill();
        var c = drawColor;
        c.a = drawOverlay.color.a;
        drawOverlay.color = c;

        drawTween = drawOverlay.DOFade(Mathf.Clamp01(to), Mathf.Max(0.01f, dur))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        return drawTween;
    }

    public void ResetOverlayAlpha()
    {
        if (drawOverlay != null)
        {
            var c = drawColor;
            c.a = 0f;
            drawOverlay.color = c;
            drawOverlay.raycastTarget = false;
        }
    }
}
