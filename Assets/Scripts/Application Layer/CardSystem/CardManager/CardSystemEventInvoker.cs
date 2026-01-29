using System;
using UnityEngine;

public class CardSystemEventInvoker
{
    public delegate void CardManagerDelegate(CardLogicSystemEventData data,ReadOnlySpan<CardDataInstance> cards);
    public CardManagerDelegate CardManagerEvent;

    public delegate void CardDataControlManagerDelegate(CardDataControlSystemEventData data, ReadOnlySpan<CardDataInstance> cards);
    public CardDataControlManagerDelegate CardDataControlManagerEvent;

    public void Dispatch(CardLogicSystemEventType type, CardSystemContextType ctx,ReadOnlySpan<CardDataInstance> cards = default)
    {
        CardLogicSystemEventData data = new CardLogicSystemEventData();
        data.contextType = ctx;
        data.eventType = type;

        CardManagerEvent?.Invoke(data, cards);
    }

    public void Dispatch(CardDataControlSystemEventType type, CardSystemContextType ctx, ReadOnlySpan<CardDataInstance> cards = default)
    {
        CardDataControlSystemEventData data = new CardDataControlSystemEventData();
        data.contextType = ctx;
        data.eventType = type;

        CardDataControlManagerEvent?.Invoke(data, cards);
    }
}
