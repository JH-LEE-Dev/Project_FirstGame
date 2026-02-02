using System;
using System.Buffers;
using System.Collections.Generic;
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


public static class ArrayPoolCSI
{
    private static Dictionary<object, string> _returnHistory = new Dictionary<object, string>();

    public static void ReportRent(object array)
    {
        if (array == null) return;
        lock (_returnHistory)
        {
            if (_returnHistory.ContainsKey(array))
            {
                _returnHistory.Remove(array);
            }
        }
    }

    public static void ReportReturn(object array)
    {
        if (array == null) return;
        lock (_returnHistory)
        {
            if (_returnHistory.ContainsKey(array))
            {
                string firstOffender = _returnHistory[array];
                string secondOffender = Environment.StackTrace;

                Debug.LogError("[이중 Dispose 검거 완료!]");
                Debug.LogError($"1️ 최초 반납 위치:\n{firstOffender}");
                Debug.LogError("--------------------------------------------------");
                Debug.LogError($"2️ 현재(중복) 반납 시도 위치:\n{secondOffender}");

                throw new Exception("이중 Dispose 발생! 콘솔을 확인하세요.");
            }
            else
            {
                _returnHistory.Add(array, Environment.StackTrace);
            }
        }
    }
}

public ref struct RentalScope<T>
{
    private T[] _array;
    public readonly Span<T> Span;
    private bool _isDisposed; // 로컬 방어용

    public RentalScope(int amount)
    {
        _array = ArrayPool<T>.Shared.Rent(amount);

        ArrayPoolCSI.ReportRent(_array);

        Span = new Span<T>(_array, 0, amount);
        _isDisposed = false;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            ArrayPoolCSI.ReportReturn(_array);
            return;
        }

        _isDisposed = true;

        // 정상 반납 신고
        ArrayPoolCSI.ReportReturn(_array);
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