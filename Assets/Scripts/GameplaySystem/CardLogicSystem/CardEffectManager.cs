using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    private IUnitLogicSystem unitLogicSystem;
    private ICardLogicSystem cardLogicSystem;

    [SerializeField] private List<CardEffectStrategy> cardEffects = new List<CardEffectStrategy>();

    public void Initialize(IUnitLogicSystem _unitLogicSystem,ICardLogicSystem _cardLogicSystem)
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
            cardEffects[(int)cardEffectTypes[i]].Execute();
        }
    }
}
