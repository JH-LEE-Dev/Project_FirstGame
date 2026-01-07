using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    private IUnitLogicSystemProvider unitLogicSystem;
    private ICardLogicSystem cardLogicSystem;

    [SerializeField] private List<CardEffectStrategy> cardEffects = new List<CardEffectStrategy>();

    public void Initialize(IUnitLogicSystemProvider _unitLogicSystem,ICardLogicSystem _cardLogicSystem)
    {
        unitLogicSystem = _unitLogicSystem;
        cardLogicSystem = _cardLogicSystem;

        for (int i = 0; i < cardEffects.Count; ++i)
        {
            cardEffects[i].Initialize(cardLogicSystem, unitLogicSystem);
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
                cardLogicSystem.StrategyForwarding(effectStrategy);
            }
            else
            {
                effectStrategy.Execute_Status();
            }
        }
    }
}
