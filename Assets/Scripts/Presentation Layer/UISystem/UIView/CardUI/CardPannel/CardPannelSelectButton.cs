using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardPannelSelectButton : ButtonInstance
{
    public Action onClickedEvent;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnHoverEnter(PointerEventData eventData)
    {

    }

    protected override void OnHoverExit(PointerEventData eventData)
    {

    }

    protected override void OnClick(PointerEventData eventData)
    {
        onClickedEvent?.Invoke();
    }
}
