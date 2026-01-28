using UnityEngine;

public class ShopUIManager : UIManager
{
    //외부 의존성
    IShopSystemData shopSystemData;
    ICardSystemData cardSystemData;

    public void Initialize(InputManager _inputManager,IShopSystemData _shopSystemData,
        ICardLocalizationSystem _cardLocalizationSystem,ICardSystemData _cardSystemData)
    {
        base.Initialize(_inputManager, _cardLocalizationSystem);

        shopSystemData = _shopSystemData;
        cardSystemData = _cardSystemData;
    }

    protected override void DataInjection(UIView view)
    {
        if (view is UIView_Shop shopUI)
            shopUI.DataInjection(shopSystemData,cardSystemData.permenantDeckCards);
    }
}
