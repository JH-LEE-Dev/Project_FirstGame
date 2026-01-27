using System;
using System.Buffers;
using static CardManager;

public ref struct RentalScope<T>
{
    private T[] _array;

    public readonly Span<T> Span;

    public RentalScope(int amount)
    {
        _array = ArrayPool<T>.Shared.Rent(amount);
        Span = new Span<T>(_array, 0, amount);
    }

    public void Dispose() => ArrayPool<T>.Shared.Return(_array, true);
}

public enum CardSystemEventType
{
    CardPileDrawEvent,
    CardAdditionalDrawEvent,
    GraveCardsToDeckEvent,
    HandCardsToGraveEvent,
    GraveCardsToHandEvent,
    CardsToExtinctionEvent,
    CardsToGraveEvent,
    ExtinctionCardsToDeckEvent,
    CardsToHandEvent,
    CardsToDeckEvent,
    MAX,
}