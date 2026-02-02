using System.Collections.Generic;
using UnityEngine;

public interface IShopSystemData
{
    IReadOnlyList<ICardDataInstanceProvider> cardMerchandiseData { get; }
}
