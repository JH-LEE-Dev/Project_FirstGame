using UnityEngine;

public interface ICardDataInstanceProvider
{
    ICardDataProvider GetCardDataProvider();

    bool IsUpgraded();
}
