using DG.Tweening;
using UnityEngine;

public class ShopCardMotion : MonoBehaviour
{
    private ShopCardInstance owner;
    private RectTransform rt;

    private Vector3 originScale;

    private Tween moveTween;
    private Tween scaleTween;

    public void Bind(ShopCardInstance card)
    {
        owner = card;
        rt = GetComponent<RectTransform>();
        originScale = transform.localScale;
    }

    public void SetOriginScale(Vector3 _originScale)
    {
        originScale = transform.localScale = _originScale;
    }

    public void AllKillTweens(bool bRestoreScale = true)
    {
        if (bRestoreScale) transform.localScale = originScale;

        moveTween?.Kill();
        moveTween = null;

        scaleTween?.Kill();
        scaleTween = null;
    }

    private void Update()
    {

    }

    public void ToIdle()
    {
        transform.localScale = originScale;
    }

    public void ToSelect()
    {
        Vector3 targetScale = originScale * 1.3f;
        transform.localScale = targetScale;
    }

    public void HoverOn()
    {

    }

    public void HoverOff()
    {

    }

    public void PickUpMoveTo(Vector2 targetAnchoredPos, float duration, bool useUnscaledTime = true)
    {
        if (!rt) return;

        AllKillTweens();

        transform.localScale = originScale * 0.5f;
        Vector3 targetScale = originScale;

        moveTween = rt
                .DOAnchorPos(targetAnchoredPos, Mathf.Max(0.01f, duration))
                .SetEase(Ease.OutCubic)
                .SetUpdate(useUnscaledTime)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        // 스케일 확대
        scaleTween = rt
            .DOScale(targetScale, Mathf.Max(0.01f, duration))
            .SetEase(Ease.OutCubic)
            .SetUpdate(useUnscaledTime)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

}
