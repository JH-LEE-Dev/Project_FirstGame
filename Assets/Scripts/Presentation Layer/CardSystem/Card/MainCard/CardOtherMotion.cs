using DG.Tweening;
using UnityEngine;

public class CardOtherMotion : MonoBehaviour
{
    private MainCardInstance card;
    private RectTransform rt;

    [Header("Hover Settings")]
    [SerializeField] private float hoverDuration = 0.35f;
    [SerializeField] private Vector3 hoverStartRot = Vector3.zero;
    [SerializeField] private Vector3 hoverStartScale = Vector3.zero;
    [SerializeField] private Ease hoverEase = Ease.OutExpo;

    [Header("Click Settings")]
    [SerializeField] private float clickDuration = 0.35f;
    [SerializeField] private Vector3 clickStartScale = Vector3.zero;
    [SerializeField] private Vector3 clickFinishScale = Vector3.zero;
    [SerializeField] private Ease clickEase = Ease.OutExpo;

    private Vector3 originRot = Vector3.zero;
    private Vector3 originScale = Vector3.one;

    private Sequence seq;

    public void Bind(MainCardInstance _card)
    {
        card = _card;
        rt = GetComponent<RectTransform>();

        originRot = rt.localEulerAngles;
        originScale = rt.localScale;
    }

    public void OnHover()
    {
        if (null == rt)
            return;

        CancelPrevMotion(seq);

        seq = DOTween.Sequence();

        rt.localScale = hoverStartScale;
        rt.eulerAngles = hoverStartRot;

        seq.Append(rt.DOLocalRotate(originRot, hoverDuration, RotateMode.FastBeyond360)
            .SetEase(hoverEase));

        seq.Join(rt.DOScale(originScale, hoverDuration)
            .SetEase(hoverEase));

        seq.OnComplete(OnOriginSetup);
    }

    public void ExitHover()
    {

    }

    public void OnClick(bool isSelect)
    {
        if (isSelect)
            SelectMotion();
        else
            UnSelectMotion();
    }

    private void SelectMotion()
    {
        CancelPrevMotion(seq);

        seq = DOTween.Sequence();

        rt.localScale = clickStartScale;
        rt.eulerAngles = hoverStartRot;

        seq.Append(rt.DOScale(clickFinishScale, clickDuration)
            .SetEase(clickEase));

        seq.OnComplete(OnClickSelect);
    }

    private void UnSelectMotion()
    {
        CancelPrevMotion(seq);

        seq = DOTween.Sequence();

        seq.Append(rt.DOScale(originScale, clickDuration)
            .SetEase(clickEase));

        seq.OnComplete(OnOriginSetup);
    }

    private void CancelPrevMotion(Sequence seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        seq = null;
    }

    private void OnOriginSetup()
    {
        if (null == rt)
            return;

        rt.eulerAngles = originRot;
        rt.localScale = originScale;
    }

    private void OnClickSelect()
    {
        rt.localScale = clickFinishScale;
    }

    private void OnClickUnSelect()
    {
        rt.localScale = originScale;
    }
}
