using System;
using System.Buffers;
using UnityEngine;
using static CardManager;

//public ref struct RentalScope<T>
//{
//    private T[] _array;

//    public readonly Span<T> Span;

//    public RentalScope(int amount)
//    {
//        _array = ArrayPool<T>.Shared.Rent(amount);
//        Span = new Span<T>(_array, 0, amount);
//    }

//    public void Dispose() => ArrayPool<T>.Shared.Return(_array, true);
//}

public ref struct RentalScope<T>
{
    private T[] _array;
    public readonly Span<T> Span;
    private bool _isDisposed; 

    public RentalScope(int amount)
    {
        _array = ArrayPool<T>.Shared.Rent(amount);
        Span = new Span<T>(_array, 0, amount);
        _isDisposed = false;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        Debug.LogWarning("Rental Buffer РЬСп Dispose!");
        
        _isDisposed = true;
        ArrayPool<T>.Shared.Return(_array, true);
    }
}

public enum CardLogicSystemEventType
{
    CardPileDrawEvent,
    CardAdditionalDrawEvent,
    GraveCardsToDeckEvent,
    HandCardsToGraveEvent,
    GraveCardsToHandEvent,
    CardsToExtinctionEvent,
    CardsToGraveEvent,
    CardsToHandEvent,
    CardsToDeckEvent,
    ExtinctionCardsToDeckEvent,
    MAX,
}

public enum CardDataControlSystemEventType
{
    CardsUpgraded,
    CardsValueModified,
    MAX,
}