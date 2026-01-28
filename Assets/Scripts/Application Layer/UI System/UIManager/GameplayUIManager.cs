using UnityEngine;

public class GameplayUIManager : UIManager
{
    //외부 의존성
    ICardSystemData cardSystemData;
    IWaveSystemData waveSystemData;

    public void Initialize(InputManager inputManager, ICardSystemData _cardSystemData,
        IWaveSystemData _waveSystemData)
    {
        base.Initialize(inputManager);

        cardSystemData = _cardSystemData;
        waveSystemData = _waveSystemData;
    }

    protected override void DataInjection(UIView view)
    {
        if (view is UIView_CardSystem cardUI)
            cardUI.DataInjection(cardSystemData.deckCards, cardSystemData.handCards, cardSystemData.graveCards,cardSystemData.extinctionCards);

        if (view is UIView_HUD hudUI)
            hudUI.DataInjection(waveSystemData);

        if (view is UIView_Unit_World unitWorldUI)
            unitWorldUI.DataInjection();
    }
}
