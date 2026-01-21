using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

using Sequence = DG.Tweening.Sequence;

public class TurnEndButton : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Main Settings")]
    [SerializeField] private RectTransform [] visualRects;

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

    [Space, Header("Up Options")]
    [ShowIf("onUp"), SerializeField] private float upDuration = 1f;
    [ShowIf("onUp"), SerializeField] private Vector3 upStartScale = Vector3.one;
    [ShowIf("onUp"), SerializeField] private Ease upEase = Ease.Linear;

    private bool clicked = false;

    private Action onCompleteAction;

    private Sequence seq;

    private List<Vector3> originScales = new();

    private void Awake()
    {
        int RectCount = visualRects.Count();

        Debug.Log(RectCount);

        for (int i = 0; i < RectCount; i++)
            originScales.Add(visualRects[i].localScale);
    }

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

        visualRects[selectIdx].localScale = originScales[selectIdx];

        seq.Append(visualRects[selectIdx].DOScale(enterTargetScale, enterDuration)
            .SetEase(enterEase));

        seq.SetUpdate(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clicked)
            return;

        int selectIdx = (int)RectSelect.Top;
        if (selectIdx >= visualRects.Count())
            return;

        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        seq.Append(visualRects[selectIdx].DOScale(originScales[selectIdx], exitDuration)
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
            visualRects[selectIdx].localScale = originScales[selectIdx];
            clicked = false;
            onCompleteAction?.Invoke();
        });

        seq.SetUpdate(false);

        clicked = true;
    }
}
