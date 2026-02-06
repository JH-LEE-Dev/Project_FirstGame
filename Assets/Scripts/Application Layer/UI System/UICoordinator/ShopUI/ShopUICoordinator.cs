using System;
using System.Collections.Generic;
using UnitSpawnSystemSignals;

public class ShopUICoordinator
{
    public event Action<List<ICardDataInstanceProvider>, ShopBehaviorType> ShopOutputEvent;
    public event Action ShopIsClosedEvent;
    public event Action CardPackRerollEvent;
    public event Action<int> ShopBillingEvent;

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

        shopUI.ShopBillingEvent -= ShopBilling;
        shopUI.ShopBillingEvent += ShopBilling;
    }

    private void ReleaseEvents()
    {
        shopUI.ShopIsClosedEvent -= ShopIsClosed;

        shopUI.CardPackRerollEvent -= CardPackReroll;

        shopUI.ShopUIOutputEvent -= ShopOutput;

        shopUI.ShopBillingEvent -= ShopBilling;
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

    private void ShopOutput(List<ICardDataInstanceProvider> _cards, ShopBehaviorType _type)
    {
        ShopOutputEvent?.Invoke(_cards, _type);
    }

    private void ShopBilling(int usedMoney)
    {
        ShopBillingEvent?.Invoke(usedMoney);
    }
}
