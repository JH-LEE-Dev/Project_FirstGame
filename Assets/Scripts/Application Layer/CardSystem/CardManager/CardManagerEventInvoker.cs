using System;
using UnityEngine;

public class CardManagerEventInvoker
{
    public delegate void CardManagerDelegate(CardSystemEventData data,ReadOnlySpan<CardDataInstance> cards);
    public CardManagerDelegate CardManagerEvent;

    public void Dispatch(CardSystemEventType type, CardSystemContextType ctx,ReadOnlySpan<CardDataInstance> cards = default)
    {
        CardSystemEventData data = new CardSystemEventData();
        data.contextType = ctx;
        data.eventType = type;

        CardManagerEvent?.Invoke(data, cards);
    }
}
