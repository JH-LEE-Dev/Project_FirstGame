using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DimOverlay : MonoBehaviour
{
    [SerializeField] private Image dimOverlay;
    private float fadeDuration = 0.85f;
    private float dimAlpha = 0.9f;

    private Tween fadeTween;

    private void Awake()
    {
        SetAlphaImmediate(0f);
        dimOverlay.raycastTarget = false;
    }


    public void SetDimOverlayActive(bool value)
    {

        fadeTween?.Kill();
        fadeTween = null;

        dimOverlay.raycastTarget = value;

        if (value)
        {
            fadeTween = dimOverlay.DOFade(dimAlpha, fadeDuration)
                .SetEase(Ease.OutCubic);
        }
        else
        {
            fadeTween = dimOverlay.DOFade(0f, fadeDuration)
                .SetEase(Ease.OutCubic);
        }
    }

    private void SetAlphaImmediate(float alpha)
    {
        Color c = dimOverlay.color;
        c.a = alpha;
        dimOverlay.color = c;
    }
}
