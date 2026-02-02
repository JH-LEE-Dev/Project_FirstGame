using UnityEngine;

public interface ICardDataInstanceProvider
{
    CardData GetCardData();

    bool IsUpgraded();
}
