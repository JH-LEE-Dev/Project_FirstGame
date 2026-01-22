using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

using Sequence = DG.Tweening.Sequence;

public class TurnEndButton : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Main Settings")]
    [SerializeField] private RectTransform visualRect;

    [Space, Header("Show Activate Options")]
    [SerializeField] private ShowOption option;

    private bool onEnter => ShowOption.OnEnter == option;
    private bool onExit => ShowOption.OnExit == option;
    private bool onUp => ShowOption.OnUp == option;

    [Space, Header("Enter Options")]
    [ShowIf("onEnter"), SerializeField] private float enterDuration = 1f;
    [ShowIf("onEnter"), SerializeField] private Vector3 enterTargetScale = Vector3.one;
    [ShowIf("onEnter"), SerializeField] private Ease enterEase = Ease.Linear;

    [Space, Header("Exit Options")]
    [ShowIf("onExit"), SerializeField] private float exitDuration = 1f;
    [ShowIf("onExit"), SerializeField] private Ease exitEase = Ease.Linear;

    [Space, Header("Down Options")]
    [ShowIf("onDown"), SerializeField] private float downDuration = 1f;
    [ShowIf("onDown"), SerializeField] private Vector3 downStartScale = Vector3.one;
    [ShowIf("onDown"), SerializeField] private Ease downEase = Ease.Linear;

    private bool clicked = false;

    private Action onCompleteAction;

    private Sequence seq;

    private Vector3 originScale;

    private void Awake()
    {
        if (visualRect)
        {
            originScale = visualRect.localScale;
        }
    }

    private void OnDisable()
    {
        seq.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        visualRect.localScale = originScale;

        seq.Append(visualRect.DOScale(enterTargetScale, enterDuration)
            .SetEase(enterEase));

        seq.SetUpdate(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clicked)
            return;

        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        seq.Append(visualRect.DOScale(originScale, exitDuration)
            .SetEase(exitEase));

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

    public void OnPointerClick(PointerEventData eventData)
    {
        onCompleteAction?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        visualRect.eulerAngles = Vector3.zero;
        visualRect.localScale = downStartScale;

        seq.Append(visualRect.DOScale(Vector3.one, downDuration)
            .SetEase(downEase));

        seq.OnComplete(() =>
        {
            visualRect.localScale = originScale;
            clicked = false;
        });

        seq.SetUpdate(false);

        clicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
