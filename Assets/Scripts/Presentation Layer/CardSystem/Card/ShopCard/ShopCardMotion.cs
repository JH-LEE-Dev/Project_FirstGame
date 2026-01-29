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
        Vector3 targetScale = originScale * 2f;
        transform.localScale = targetScale;
    }

    public void ToSelect()
    {
        Vector3 targetScale = originScale * 2.5f;
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

        Vector3 targetScale = originScale * 2f;

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
