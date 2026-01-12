using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectCommandManager : MonoBehaviour
{
    public event Action<CardEffectSystemCommand> CardEffectSystemCommandDispatchEvent;
    public event Action<CardEffectStatusCommand> CardEffectStatusCommandDispatchEvent;

    [SerializeField] private List<CardEffectStatusCommand> cardStatusCommands = new List<CardEffectStatusCommand>();
    [SerializeField] private List<CardEffectSystemCommand> cardSystemCommands = new List<CardEffectSystemCommand>();

    public void AnalysisCardEffect(CardDataInstance card)
    {
        List<CardStatusEffectType> cardStatusEffectTypes = card.GetCardData().cardStatusEffects;
        List<CardSystemEffectType> cardSystemEffectTypes = card.GetCardData().cardSystemEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectStatusCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];
            CardEffectStatusCommandDispatchEvent?.Invoke(effectCommand);
        }

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectSystemCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];
            CardEffectSystemCommandDispatchEvent?.Invoke(effectCommand);
        }
    }
}
