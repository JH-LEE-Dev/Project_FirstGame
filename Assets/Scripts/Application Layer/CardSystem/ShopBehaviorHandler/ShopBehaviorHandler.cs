using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class ShopBehaviorHandler
{
    public delegate void RequestCardSystemActionDelegate(CardLogicSystemActionType type, ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    public RequestCardSystemActionDelegate RequestCardLogicSystemActionEvent;
    public delegate void RequestCardDataControlSystemActionDelegate(CardDataControlSystemActionType type, ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    public RequestCardDataControlSystemActionDelegate RequestCardDataControlSystemActionEvent;

    //외부 의존성
    ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler; //->여기에 의존하면 안됨. 구조 고치기.

    public void Initialize(ICardLogicSystemActionCommandHandler _cardLogicSystemActionCommandHandler)
    {
        cardLogicSystemActionCommandHandler =_cardLogicSystemActionCommandHandler;
    }

    public void AnalysisShopBehavior(List<ICardDataInstanceProvider> _cards, ShopBehaviorType _type)
    {
        if (_cards.Count == 0)
            return;

        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        if (_type == ShopBehaviorType.Delete)
        {
            cardLogicSystemActionCommandHandler.DeleteCardsFromDeck(writeBuffer);
        }

        if (_type == ShopBehaviorType.Upgrade)
        {
            RequestCardDataControlSystemActionEvent?.Invoke(CardDataControlSystemActionType.CardsPermenantlyUpgraded, writeBuffer, CardSystemContextType.MAX);
        }

        if (_type == ShopBehaviorType.PickUp)
        {
            cardLogicSystemActionCommandHandler.AddCardsToDeck(writeBuffer);
        }
    }
}
