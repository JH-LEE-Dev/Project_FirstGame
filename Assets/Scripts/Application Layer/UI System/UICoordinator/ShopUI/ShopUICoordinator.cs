using System;
using System.Collections.Generic;
using UnitSpawnSystemSignals;

public class ShopUICoordinator
{
    public event Action<List<CardDataInstance>, ShopBehaviorType> ShopOutputEvent;
    public event Action ShopIsClosedEvent;
    public event Action CardPackRerollEvent;

    private UIView_Shop shopUI;

    public void Initialize(UIView_Shop _shopUI)
    {
        shopUI = _shopUI;

        BindEvents();
    }

    private void BindEvents()
    {
        shopUI.ShopIsClosedEvent -= ShopIsClosed;
        shopUI.ShopIsClosedEvent += ShopIsClosed;

        shopUI.CardPackRerollEvent -= CardPackReroll;
        shopUI.CardPackRerollEvent += CardPackReroll;

        shopUI.ShopUIOutputEvent -= ShopOutput;
        shopUI.ShopUIOutputEvent += ShopOutput;
    }

    private void ReleaseEvents()
    {
        shopUI.ShopIsClosedEvent -= ShopIsClosed;

        shopUI.CardPackRerollEvent -= CardPackReroll;

        shopUI.ShopUIOutputEvent -= ShopOutput;
    }

    public void ShopOpened()
    {
        shopUI.Show();
        shopUI.OpenShop();
    }

    public void ShopIsClosed()
    {
        ShopIsClosedEvent?.Invoke();
        shopUI.Hide();
    }

    public void Release()
    {
        ReleaseEvents();
    }

    private void CardPackReroll()
    {
        CardPackRerollEvent?.Invoke();
    }

    private void ShopOutput(List<CardDataInstance> _cards, ShopBehaviorType _type)
    {
        ShopOutputEvent?.Invoke(_cards, _type);
    }
}
