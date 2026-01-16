using CardEffectSystemSignal;
using CardSystemSignals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectCommandManager : MonoBehaviour
{
    //외부 의존성
    private ISignalHub<IPulicSignal> signalHub;

    [SerializeField] private List<CardEffectStatusCommand> cardStatusCommands = new List<CardEffectStatusCommand>();
    [SerializeField] private List<CardEffectSystemCommand> cardSystemCommands = new List<CardEffectSystemCommand>();

    public void Initialize(ISignalHub<IPulicSignal> _signalHub)
    {
        signalHub = _signalHub;

        SubscribeEvent();
    }

    private void SubscribeEvent()
    {
        signalHub.Subscribe<CardUsedEvent>(AnalysisCardEffect);
    }

    private void UnSubscribeEvent()
    {
        signalHub.UnSubscribe<CardUsedEvent>(AnalysisCardEffect);
    }

    public void AnalysisCardEffect(CardUsedEvent cardUsedEvent)
    {
        CardDataInstance usedCard = cardUsedEvent.usedCard;

        List<CardStatusEffectType> cardStatusEffectTypes = usedCard.GetCardData().cardStatusEffects;
        List<CardSystemEffectType> cardSystemEffectTypes = usedCard.GetCardData().cardSystemEffects;

        for (int i = 0; i < cardStatusEffectTypes.Count; ++i)
        {
            CardEffectStatusCommand effectCommand = cardStatusCommands[(int)cardStatusEffectTypes[i]];

            signalHub.Publish(new CardEffectStatusCommandDispatchEvent(effectCommand));
        }

        for (int i = 0; i < cardSystemEffectTypes.Count; ++i)
        {
            CardEffectSystemCommand effectCommand = cardSystemCommands[(int)cardSystemEffectTypes[i]];

            signalHub.Publish(new CardEffectSystemCommandDispatchEvent(effectCommand));
        }
    }

    public void Release()
    {
        UnSubscribeEvent();
    }
}
