using UnityEngine;

public class GameplayUIManager : UIManager
{
    //외부 의존성
    ICardSystemData cardSystemData;

    public void Initialize(InputManager inputManager, ICardSystemData _cardSystemData)
    {
        base.Initialize(inputManager);

        cardSystemData = _cardSystemData;
    }

    protected override void DataInjection(UIView view)
    {
        if (view is UIView_CardSystem cardUI)
            cardUI.DataInjection(cardSystemData.deckCards, cardSystemData.handCards, cardSystemData.graveCards);

        if (view is UIView_HUD hudUI)
            hudUI.DataInjection();

        if (view is UIView_Unit unitUI)
            unitUI.DataInjection();
    }
}
