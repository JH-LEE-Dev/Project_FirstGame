using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class SelectEndButton : ButtonInstance
{
    private HandSystem owner;

    public void Init(HandSystem _owner)
    {
        base.Init();
        owner = _owner;
    }

    // 상태 바뀔 때. 연출할수도있음.
    protected override void OnStateChanged(VisualState oldState, VisualState newState)
    {

    }

    // 호버 ON 구현
    protected override void OnHoverEnter(PointerEventData eventData)
    {

    }

    // 호버 OFF 구현
    protected override void OnHoverExit(PointerEventData eventData)
    {

    }

    // 클릭.
    protected override void OnClick(PointerEventData eventData)
    {
        owner.EndCardSelectMode();
    }
}
