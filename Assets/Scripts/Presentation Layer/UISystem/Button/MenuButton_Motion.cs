using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

enum ShowOption
{
    OnEnter,
    OnUp,
    OnDown
};

enum RectSelect
{
    Top,
    Middle,
    Bottom,
};

public class MenuButton_Motion : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Main Settings")]
    [SerializeField] private RectTransform[] visualRects;

    [Space, Header("Show Activate Options")]
    [SerializeField] private ShowOption option;

    private bool onEnter => ShowOption.OnEnter == option;
    private bool onUp => ShowOption.OnUp == option;
    private bool onDown => ShowOption.OnDown == option;

    [Space, Header("Enter Options")]
    [ShowIf("onEnter"), SerializeField] private float enterDuration = 1f;
    [ShowIf("onEnter"), SerializeField] private Vector3 enterStartRot = Vector3.zero;
    [ShowIf("onEnter"), SerializeField] private Ease enterEase = Ease.Linear;

    [Space, Header("Up Options")]
    [ShowIf("onUp"), SerializeField] private float upDuration = 1f;

    [Space, Header("Down Options")]
    [ShowIf("onDown"), SerializeField] private float downDuration = 1f;

    private Sequence seq;

    public void OnPointerEnter(PointerEventData eventData)
    {
        int selectIdx = (int)RectSelect.Top;
        if (selectIdx >= visualRects.Count())
            return;

        CancelPrevMotion(seq);
        seq = DOTween.Sequence();

        visualRects[selectIdx].eulerAngles = enterStartRot; 

        seq.Append(visualRects[selectIdx].DORotate(Vector3.zero, enterDuration, RotateMode.FastBeyond360)
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
        
    }

    private void CancelPrevMotion(Sequence _seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();
    }
}
