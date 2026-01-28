using System.Collections.Generic;
using UnityEngine;

public interface IShopSystemData
{
    IReadOnlyList<CardData> cardMerchandiseData { get; }
}
