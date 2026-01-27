using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIView_Shop : UIView
{
    private const int initialcardMerchandiseCnt = 5;

    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private Button buyButton_1;
    [SerializeField] private Button buyButton_2;
    [SerializeField] private Button buyButton_3;
    [SerializeField] private Button buyButton_4;
    [SerializeField] private Button buyButton_5;
    [SerializeField] private Button shopCloseButton;

    private List<CardData> cardDataList =new List<CardData>(initialcardMerchandiseCnt);

    public override void Initialize(UIViewContext ctx)
    {
        base.Initialize(ctx);
    }

    public void OpenShop()
    {
        PrepareMerchandise();
    }

    public void CloseShop()
    {
        cardDataList.Clear();
    }

    private void PrepareMerchandise()
    {
        for (int i = 0; i < initialcardMerchandiseCnt; ++i)
        {
            int randomIdx = UnityEngine.Random.Range(0, cardDataBase.cardData.Count - 1);

            CardData cardData = cardDataBase.cardData[randomIdx];

            cardDataList.Add(cardData);
        }
    }
}
