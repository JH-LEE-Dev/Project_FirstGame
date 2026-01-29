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
    [Header("Buttons")]
    [SerializeField] private Button pickUpCardButton;
    [SerializeField] private Button enforceCardButton;
    [SerializeField] private Button deleteCardButton;
    [SerializeField] private Button viewDeckButton;
    [SerializeField] private Button nextStageButton;

    private bool EnforcedComplete = false;
    private bool DeletedComplete = false;

    [Header("System")]
    private ShopPoolingSystem shopPoolingSystem;


    public override void Initialize(UIViewContext ctx)
    {
        base.Initialize(ctx);

        SafeBind(pickUpCardButton, OnClick_PickUpCard);
        SafeBind(enforceCardButton, OnClick_EnforceCard);
        SafeBind(deleteCardButton, OnClick_DeleteCard);
        SafeBind(viewDeckButton, OnClick_ViewDeck);
        SafeBind(nextStageButton, OnClick_NextStage);

        if (!shopPoolingSystem) shopPoolingSystem = GetComponent<ShopPoolingSystem>();
    }
    private void SafeBind(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (!btn)
        {
            Debug.LogWarning($"{nameof(UIView_Shop)}: Button reference missing for {action.Method.Name}");
            return;
        }
        btn.onClick.AddListener(action);
    }

    public void DataInjection(IShopSystemData _shopSystemData, IReadOnlyList<CardDataInstance> _deckCards)
    {
        shopSystemData = _shopSystemData;
    }

    public void OpenShop()
    {


    }










    ////////////// PoolingCard

    public ShopCardInstance RentCard(CardDataInstance data)
    {
        var card = shopPoolingSystem?.RentCard();
        card.ApplyData(data);
        // 알아서 Active On
        return card;
    }
    public void ReturnCard(ShopCardInstance card)
    {
        // 알아서 Active Off, Data 초기화
        shopPoolingSystem?.ReturnCard(card);
    }


    ////////////// Click
    private void OnClick_PickUpCard()
    {
        Debug.Log("[Shop] PickUpCard clicked");

    }

    private void OnClick_EnforceCard()
    {
        Debug.Log("[Shop] EnforceCard clicked");
        // TODO: 강화 로직

    }

    private void OnClick_DeleteCard()
    {
        Debug.Log("[Shop] DeleteCard clicked");
        // TODO: 삭제 로직


    }
    private void OnClick_ViewDeck()
    {
        Debug.Log("[Shop] ViewDeck clicked");
        // TODO: 덱 보기 UI 열기


    }
    private void OnClick_NextStage()
    {
        Debug.Log("[Shop] NextStage clicked");


    }



    // For PickUpCard

    // For EnforceCard

    // For DeleteCard

    // For ViewDeck

    // For NextStage
}
