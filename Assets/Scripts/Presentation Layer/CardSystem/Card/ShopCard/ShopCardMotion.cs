using DG.Tweening;
using UnityEngine;

public class ShopCardMotion : MonoBehaviour
{
    private ShopCardInstance owner;
    private RectTransform rt;

    private Vector3 originScale;

    private Tween moveTween;

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
    }

    private void Update()
    {

    }

    public void ToIdle()
    {

    }

    public void ToSelect()
    {

    }

    public void HoverOn()
    {

    }

    public void HoverOff()
    {

    }

    public void MoveTo(Vector2 targetAnchoredPos, float duration, bool useUnscaledTime = true)
    {
        if (!rt) return;

        AllKillTweens();

        moveTween = rt
                    .DOAnchorPos(targetAnchoredPos, Mathf.Max(0.01f, duration))
                    .SetEase(Ease.OutCubic)
                    .SetUpdate(useUnscaledTime)
                    .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

}
