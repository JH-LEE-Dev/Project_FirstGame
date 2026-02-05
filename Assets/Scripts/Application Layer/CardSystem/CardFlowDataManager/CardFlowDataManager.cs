using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CardFlowDataManager : ICardFlowDataActionCommandHandler
{
    public delegate void CardLogicSystemCommandCreator(GameSystemActionContextType cardSystemContextType, ReadOnlySpan<CardDataInstance> cards);
    private Dictionary<CardLogicSystemEventType, CardLogicSystemCommandCreator> cardLogicSystemCreatorMap = new();

    private List<CardDataInstance> prevTurnHandToGraveCards = new List<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);

    public void Initialize()
    {
        cardLogicSystemCreatorMap = new Dictionary<CardLogicSystemEventType, CardLogicSystemCommandCreator>();

        BindLogic(CardLogicSystemEventType.HandCardsToGraveEvent, HandCardsToGrave);

        void BindLogic(CardLogicSystemEventType type, CardLogicSystemCommandCreator action)
            => cardLogicSystemCreatorMap[type] = action;
    }

    public void CatchCardFlow(CardLogicSystemEventData cardLogicSystemEventData, ReadOnlySpan<CardDataInstance> cards = default)
    {
        if (cardLogicSystemCreatorMap.TryGetValue(cardLogicSystemEventData.eventType, out var creator))
        {
            creator.Invoke(cardLogicSystemEventData.contextType, cards);
        }
    }

    public void HandCardsToGrave(GameSystemActionContextType cardSystemContextType, ReadOnlySpan<CardDataInstance> cards)
    {
        prevTurnHandToGraveCards.Clear();

        for (int i = 0; i < cards.Length; ++i)
        {
            prevTurnHandToGraveCards.Add(cards[i]);
        }
    }

    public IReadOnlyList<CardDataInstance> GetPrevTurnHandToGraveCards()
    {
        return prevTurnHandToGraveCards;
    }
}
