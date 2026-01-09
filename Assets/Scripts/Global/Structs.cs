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
}

public struct Job_CardSystemUI
{
    public JobType_CardSystemUI jobType;
    public List<CardDataInstance> cards;
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