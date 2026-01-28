using UnityEngine;

public class ShopUIManager : UIManager
{
    //외부 의존성
    IShopSystemData shopSystemData;

    public void Initialize(InputManager inputManager,IShopSystemData _shopSystemData)
    {
        base.Initialize(inputManager);

        shopSystemData = _shopSystemData;
    }

    protected override void DataInjection(UIView view)
    {
        if (view is UIView_Shop shopUI)
            shopUI.DataInjection(shopSystemData);
    }
}
