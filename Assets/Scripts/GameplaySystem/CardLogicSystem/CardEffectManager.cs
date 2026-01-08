using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    private IUnitLogicSystemProvider unitLogicSystem;
    private ICardStrategyHandler cardStrategyHandler;

    [SerializeField] private List<CardEffectStrategy> cardEffects = new List<CardEffectStrategy>();

    public void Initialize(IUnitLogicSystemProvider _unitLogicSystem,ICardStrategyHandler _cardStrategyHandler)
    {
        unitLogicSystem = _unitLogicSystem;
        cardStrategyHandler = _cardStrategyHandler;

        for (int i = 0; i < cardEffects.Count; ++i)
        {
            cardEffects[i].Initialize(cardStrategyHandler, unitLogicSystem);
        }
    }

    public void ExecuteCardEffect(CardData cardData)
    {
        List<CardEffectType> cardEffectTypes = cardData.cardEffects;

        for(int i = 0; i < cardEffectTypes.Count; ++i)
        {
            CardEffectStrategy effectStrategy = cardEffects[(int)cardEffectTypes[i]];

            if(effectStrategy.GetCardEffectApplyType() == CardEffectApplyType.System)
            {
                cardStrategyHandler.StrategyForwarding(effectStrategy);
            }
            else
            {
                effectStrategy.Execute_Status();
            }
        }
    }
}
