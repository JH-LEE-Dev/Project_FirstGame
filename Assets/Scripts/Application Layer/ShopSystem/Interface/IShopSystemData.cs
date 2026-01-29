using System.Collections.Generic;
using UnityEngine;

public interface IShopSystemData
{
    IReadOnlyList<CardDataInstance> cardMerchandiseData { get; }
}
