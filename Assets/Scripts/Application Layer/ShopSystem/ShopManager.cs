using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopManager : MonoBehaviour, IShopSystemData
{
    IReadOnlyList<CardDataInstance> IShopSystemData.cardMerchandiseData => cardMerchandiseData;

    //외부 의존성
    private ICardLogicSystemProvider cardLogicSystemProvider;

    public event Action ShopIsReadyEvent;

    private const int initialcardMerchandiseCnt = 5;

    [Header("Card Data")]
    [SerializeField] private CardDataBase cardDataBase;

    private List<CardData> cardDataMerchandiseData = new List<CardData>(initialcardMerchandiseCnt);
    private List<CardDataInstance> cardMerchandiseData = new List<CardDataInstance>(initialcardMerchandiseCnt);

    public void Initialize(ICardLogicSystemProvider _cardLogicSystemProvider)
    {
        cardLogicSystemProvider = _cardLogicSystemProvider;
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
        ReleaseMerchandise();
    }

    private void ReleaseMerchandise()
    {
        cardDataMerchandiseData.Clear();

        for(int i = 0;i<cardMerchandiseData.Count;++i)
        {
            cardLogicSystemProvider.ReleaseCard(cardMerchandiseData[i]);
        }

        cardMerchandiseData.Clear();
    }

    private void PrepareMerchandise()
    {
        for (int i = 0; i < initialcardMerchandiseCnt; ++i)
        {
            int randomIdx = UnityEngine.Random.Range(0, cardDataBase.cardData.Count - 1);

            CardData cardData = cardDataBase.cardData[randomIdx];

            cardDataMerchandiseData.Add(cardData);
            cardMerchandiseData.Add(cardLogicSystemProvider.CreateCard(cardData.id));
        }
    }

    public void RerollMerchandise()
    {
        cardDataMerchandiseData.Clear();

        for (int i = 0; i < cardMerchandiseData.Count; ++i)
        {
            cardLogicSystemProvider.ReleaseCard(cardMerchandiseData[i]);
        }

        cardMerchandiseData.Clear();

        for (int i = 0; i < initialcardMerchandiseCnt; ++i)
        {
            int randomIdx = UnityEngine.Random.Range(0, cardDataBase.cardData.Count - 1);

            CardData cardData = cardDataBase.cardData[randomIdx];

            cardDataMerchandiseData.Add(cardData);
            cardMerchandiseData.Add(cardLogicSystemProvider.CreateCard(cardData.id));
        }
    }
}
