using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Buffers;

[Serializable]
public struct CanvasRoot
{
    public Transform screenLayerRoot;
    public Transform popupLayerRoot;
    public Transform overlayLayerRoot;
    public Transform tooltipLayerRoot;
    public Transform worldLayerRoot;
}

public struct ActionData_CardSystem
{
    public ActionType_CardSystem actionDataType;
    public List<CardDataInstance> cards;
}

public struct ActionDataBatch_CardSystem
{
    public List<ActionData_CardSystem> actionDataList;
    public int idx;
}


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

public struct CardUsingResult
{
    CardDataInstance usedCard;
    int slotIdx;

    public CardUsingResult(CardDataInstance _usedCard, int _slotIdx)
    {
        usedCard = _usedCard;
        slotIdx = _slotIdx;
    }
}
