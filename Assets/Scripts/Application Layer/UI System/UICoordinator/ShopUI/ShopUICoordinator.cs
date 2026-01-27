using UnityEngine;

public class ShopUICoordinator
{
    private UIView_Shop shopUI;

    public void Initialize(UIView_Shop _shopUI)
    {
        shopUI = _shopUI;
    }

    public void ShopOpened()
    {
        shopUI.OpenShop();
    }
}
