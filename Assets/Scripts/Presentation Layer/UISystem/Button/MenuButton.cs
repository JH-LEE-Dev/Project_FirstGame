using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Main Settings")]
    [SerializeField] private RectTransform visualRect;
    [SerializeField] private TMP_Text mainText;

    [Space, Header("Show Activate Options")]
    [SerializeField] private ShowOption option;

    private bool onEnter => ShowOption.OnEnter == option;
    private bool onExit => ShowOption.OnExit == option;
    private bool onDown => ShowOption.OnDown == option;

    [Space, Header("Enter Options")]
    [ShowIf("onEnter"), SerializeField] private float enterDuration = 1f;
    [ShowIf("onEnter"), SerializeField] private Vector3 enterStartScale = Vector3.one;
    [ShowIf("onEnter"), SerializeField] private float enterMoveDistance = 5f;
    [Space]
    [ShowIf("onEnter"), SerializeField] private Color enterColor = Color.white;
    [Space]
    [ShowIf("onEnter"), SerializeField] private Ease enterEase = Ease.Linear;

    [Space, Header("Exit Options")]
    [ShowIf("onExit"), SerializeField] private float exitDuration = 1f;
    [ShowIf("onExit"), SerializeField] private Ease exitEase = Ease.Linear;

    [Space, Header("Down Options")]
    [ShowIf("onDown"), SerializeField] private float downDuration = 1f;
    [ShowIf("onDown"), SerializeField] private Vector3 downStartScale = Vector3.one;
    [ShowIf("onDown"), SerializeField] private Ease downEase = Ease.Linear;

    private Action onCompleteAction;
    private Sequence seq;

    private Color originColor;
    private Vector3 originAnchoredPos;
    private Vector3 originScale;

    private void Awake()
    {
        if (visualRect)
        {
            originScale = visualRect.localScale;
            originAnchoredPos = visualRect.anchoredPosition;
        }

        if (mainText)
        {
            originColor = mainText.color;
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

        visualRect.localScale = enterStartScale; 

        seq.Append(visualRect.DOScale(originScale, enterDuration)
            .SetEase(enterEase));

        Vector3 newPos = visualRect.anchoredPosition + (Vector2.right * enterMoveDistance);

        seq.Join(visualRect.DOAnchorPos(newPos, enterDuration)
            .SetEase(enterEase));

        seq.Join(mainText.DOColor(enterColor, enterDuration)
            .SetEase(enterEase));

        seq.OnComplete(ResetTextAndLocalScale);

        seq.SetUpdate(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        seq.Append(visualRect.DOScale(originScale, enterDuration)
            .SetEase(enterEase));

        seq.Join(visualRect.DOAnchorPos(originAnchoredPos, exitDuration)
            .SetEase(exitEase));

        seq.Join(mainText.DOColor(originColor, exitDuration)
            .SetEase(exitEase));

        seq.OnComplete(ResetExitTextAndLocalScale);

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

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        visualRect.localScale = downStartScale;
        mainText.color = enterColor;

        seq.Append(visualRect.DOScale(originScale, downDuration)
            .SetEase(downEase));

        seq.OnComplete(PointerDownCompleteEvent);

        seq.SetUpdate(false);
    }

    private void ResetTextAndLocalScale()
    {
        mainText.color = enterColor;
        visualRect.localScale = originScale;
    }

    private void ResetExitTextAndLocalScale()
    {
        mainText.color = originColor;
        visualRect.localScale = originScale;
    }

    private void PointerDownCompleteEvent()
    {
        visualRect.localScale = originScale;
    }
}
