using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

enum ShowOption
{
    OnEnter,
    OnUp
};

enum RectSelect
{
    Top,
    Middle,
    Bottom,
};

public class MenuButton : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Main Settings")]
    [SerializeField] private RectTransform[] visualRects;

    [Space, Header("Show Activate Options")]
    [SerializeField] private ShowOption option;

    private bool onEnter => ShowOption.OnEnter == option;
    private bool onUp => ShowOption.OnUp == option;

    [Space, Header("Enter Options")]
    [ShowIf("onEnter"), SerializeField] private float enterDuration = 1f;
    [ShowIf("onEnter"), SerializeField] private Vector3 enterStartRot = Vector3.zero;
    [ShowIf("onEnter"), SerializeField] private Ease enterEase = Ease.Linear;

    [Space, Header("Up Options")]
    [ShowIf("onUp"), SerializeField] private float upDuration = 1f;
    [ShowIf("onUp"), SerializeField] private Vector3 upStartScale = Vector3.one;
    [ShowIf("onUp"), SerializeField] private Ease upEase = Ease.Linear;

    private Action onCompleteAction;

    private Sequence seq;

    private void OnDisable()
    {
        seq.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        int selectIdx = (int)RectSelect.Top;
        if (selectIdx >= visualRects.Count())
            return;

        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        visualRects[selectIdx].eulerAngles = enterStartRot; 

        seq.Append(visualRects[selectIdx].DOLocalRotate(Vector3.zero, enterDuration, RotateMode.FastBeyond360)
            .SetEase(enterEase));

        seq.OnComplete(() =>
        {
            visualRects[selectIdx].eulerAngles = Vector3.zero;
        });

        seq.SetUpdate(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        int selectIdx = (int)RectSelect.Top;
        if (selectIdx >= visualRects.Count())
            return;

        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        visualRects[selectIdx].eulerAngles = Vector3.zero;
        visualRects[selectIdx].localScale = upStartScale;

        seq.Append(visualRects[selectIdx].DOScale(Vector3.one, upDuration)
            .SetEase(upEase));

        seq.OnComplete(() =>
        {
            visualRects[selectIdx].localScale = Vector3.one;

            onCompleteAction?.Invoke();
        });

        seq.SetUpdate(false);
    }

    public void OnCompleteAction(Action _action)
    {
        onCompleteAction -= _action;
        onCompleteAction += _action;
    }

    private void CancelPrevMotion(Sequence _seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();
    }
}
