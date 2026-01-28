using System;

public class ShopUICoordinator
{
    public event Action ShopIsClosedEvent;

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
    }

    private void ReleaseEvents()
    {
        shopUI.ShopIsClosedEvent -= ShopIsClosed;
    }

    public void ShopOpened()
    {
        shopUI.Show();
        shopUI.OpenShop();
    }

    public void ShopIsClosed()
    {
        ShopIsClosedEvent?.Invoke();
    }

    public void Release()
    {
        ReleaseEvents();
    }
}
