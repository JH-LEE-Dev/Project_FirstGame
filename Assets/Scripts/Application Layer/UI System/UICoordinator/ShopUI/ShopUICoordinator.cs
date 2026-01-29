using System;
using UnitSpawnSystemSignals;

public class ShopUICoordinator
{
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
    }

    private void ReleaseEvents()
    {
        shopUI.ShopIsClosedEvent -= ShopIsClosed;

        shopUI.CardPackRerollEvent -= CardPackReroll;
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
}
