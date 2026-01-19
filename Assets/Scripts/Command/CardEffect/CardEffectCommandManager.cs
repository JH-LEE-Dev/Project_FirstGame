using CardEffectSystemSignal;
using CardSystemSignals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectCommandManager : MonoBehaviour
{
    public event Action<CardEffectSystemCommand> SystemCommandDispatchEvent;
    public event Action<CardEffectStatusCommand> StatusCommandDispatchEvent;

    [SerializeField] private List<CardEffectStatusCommand> cardStatusCommands = new List<CardEffectStatusCommand>();
    [SerializeField] private List<CardEffectSystemCommand> cardSystemCommands = new List<CardEffectSystemCommand>();

    public void Initialize()
    {
    }

    public void AnalysisCardEffect(CardDataInstance usedCard)
    {
        List<CardStatusEffectType> cardStatusEffectTypes = usedCard.GetCardData().cardStatusEffects;
        List<CardSystemEffectType> cardSystemEffectTypes = usedCard.GetCardData().cardSystemEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectStatusCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];

            StatusCommandDispatchEvent?.Invoke(effectCommand);
        }

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectSystemCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];

            SystemCommandDispatchEvent?.Invoke(effectCommand);
        }
    }

    public void Release()
    {
    }
}
