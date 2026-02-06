using UnityEngine;

public class GameplayUIManager : UIManager
{
    //외부 의존성
    ICardSystemData cardSystemData;
    IWaveSystemData waveSystemData;
    IUnitSpawnSystemData unitSpawnSystemData;

    public void Initialize(InputManager _inputManager, ICardSystemData _cardSystemData,
        IWaveSystemData _waveSystemData,ICardLocalizationSystem _cardLocalizationSystem,
        IUnitSpawnSystemData _unitSpawnSystemData)
    {
        base.Initialize(_inputManager, _cardLocalizationSystem);

        cardSystemData = _cardSystemData;
        waveSystemData = _waveSystemData;
        unitSpawnSystemData = _unitSpawnSystemData;
    }

    protected override void DataInjection(UIView view)
    {
        if (view is UIView_CardSystem cardUI)
            cardUI.DataInjection(cardSystemData.deckCards, cardSystemData.handCards, cardSystemData.graveCards,cardSystemData.extinctionCards);

        if (view is UIView_HUD hudUI)
            hudUI.DataInjection(waveSystemData,unitSpawnSystemData.playerData,unitSpawnSystemData.characterData);

        if (view is UIView_Unit_World unitWorldUI)
            unitWorldUI.DataInjection(unitSpawnSystemData.characterData,unitSpawnSystemData.enemiesData);

        if (view is UIView_Unit_Canvas unitCanvasUI)
            unitCanvasUI.DataInjection(unitSpawnSystemData.characterData, unitSpawnSystemData.enemiesData);
    }
}
