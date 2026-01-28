using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopManager : MonoBehaviour, IShopSystemData
{
    IReadOnlyList<CardData> IShopSystemData.cardMerchandiseData => cardMerchandiseData;

    public event Action ShopIsReadyEvent;

    private const int initialcardMerchandiseCnt = 5;

    [Header("Card Data")]
    [SerializeField] private CardDataBase cardDataBase;

    private List<CardData> cardMerchandiseData = new List<CardData>(initialcardMerchandiseCnt);

    public void Initialize()
    {

    }

    public void Release()
    {

    }

    public void OpenShop()
    {
        PrepareMerchandise();

        ShopIsReadyEvent?.Invoke();
    }

    public void CloseShop()
    {
        cardMerchandiseData.Clear();
    }

    private void PrepareMerchandise()
    {
        for (int i = 0; i < initialcardMerchandiseCnt; ++i)
        {
            int randomIdx = UnityEngine.Random.Range(0, cardDataBase.cardData.Count - 1);

            CardData cardData = cardDataBase.cardData[randomIdx];

            cardMerchandiseData.Add(cardData);
        }
    }
}
