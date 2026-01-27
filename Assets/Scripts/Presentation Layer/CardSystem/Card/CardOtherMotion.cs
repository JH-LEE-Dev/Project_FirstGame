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

    private Quaternion originRot = Quaternion.identity;
    private Vector3 originScale = Vector3.one;

    private Sequence hoverSeq;

    public void Bind(MainCardInstance _card)
    {
        card = _card;
        rt = GetComponent<RectTransform>();

        originRot = rt.localRotation;
        originScale = rt.localScale;
    }

    public void OnHover()
    {

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

    }

    private void UnSelectMotion()
    {

    }

    private void CancelPrevMotion(Sequence seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        seq = null;
    }
}
