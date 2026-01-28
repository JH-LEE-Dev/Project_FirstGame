using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIView_Shop : UIView
{
    public event Action ShopIsClosedEvent;

    //외부 의존성
    private IShopSystemData shopSystemData;

    //현재 게임 시스템의 카드 정보.
    IReadOnlyList<CardDataInstance> deckCards;

    [SerializeField] private Button buyButton_1;
    [SerializeField] private Button buyButton_2;
    [SerializeField] private Button buyButton_3;
    [SerializeField] private Button buyButton_4;
    [SerializeField] private Button buyButton_5;
    [SerializeField] private Button shopCloseButton;

    public override void Initialize(UIViewContext ctx)
    {
        base.Initialize(ctx);
    }

    public void DataInjection(IShopSystemData _shopSystemData, IReadOnlyList<CardDataInstance> _deckCards)
    {
        shopSystemData = _shopSystemData;
    }

    public void OpenShop()
    {
    
    }

    public void CloseShop()
    {

    }
}
