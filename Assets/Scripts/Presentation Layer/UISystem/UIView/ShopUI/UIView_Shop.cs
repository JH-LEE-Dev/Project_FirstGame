using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIView_Shop : UIView
{
    public event Action ShopIsClosedEvent;
    public event Action CardPackRerollEvent;
    public event Action<List<ICardDataInstanceProvider>,ShopBehaviorType> ShopUIOutputEvent;

    //외부 의존성
    private IShopSystemData shopSystemData;
    private IPlayerData playerData;

    //현재 게임 시스템의 카드 정보.
    IReadOnlyList<ICardDataInstanceProvider> deckCards;

    [Header("Buttons")]
    [SerializeField] private Button pickUpCardButton;
    [SerializeField] private Button enforceCardButton;
    [SerializeField] private Button deleteCardButton;
    [SerializeField] private Button viewDeckButton;
    [SerializeField] private Button nextStageButton;


    private int pickUpCardCount = 2;
    private bool pickUpCardForce = false;
    private int enforceCardCount = 1;
    private int deleteCardCount = 1;

    private bool EnforcedComplete = false;
    private bool DeletedComplete = false;

    [Header("System")]
    private ShopPoolingSystem shopPoolingSystem;
    private ShopSelectSystem selectSystem;

    [Header("PickUpSystem")]
    [SerializeField] private PickUpSystem pickUpSystem;

    [Header("DeckSystem")]
    [SerializeField] private ShopCardPannel cardPannel;
    [SerializeField] private ShopDeckSystem deckSystem;
    [SerializeField] private GameObject pannelContent = null;
    public GameObject PannelContent { get { return pannelContent; } }
    public ShopCardPannel CardPannel { get { return cardPannel; } }

    ///////// Basic Values
    private Vector3 pannelCardScale = new Vector3(5f, 5f, 1f);


    public override void Initialize(UIViewContext ctx)
    {
        base.Initialize(ctx);

        SafeBind(pickUpCardButton, OnClick_PickUpCard);
        SafeBind(enforceCardButton, OnClick_EnforceCard);
        SafeBind(deleteCardButton, OnClick_DeleteCard);
        SafeBind(viewDeckButton, OnClick_ViewDeck);
        SafeBind(nextStageButton, OnClick_NextStage);

        if (!shopPoolingSystem)
            shopPoolingSystem = GetComponent<ShopPoolingSystem>();
        if (!selectSystem)
            selectSystem = GetComponent<ShopSelectSystem>();

        shopPoolingSystem.Init(this, viewCtx.cardLocalizationSystem);
        selectSystem.Init(this);

        pickUpSystem?.Init(this);
        cardPannel?.Init(this);
        deckSystem?.Init(this);

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

    public void DataInjection(IShopSystemData _shopSystemData, IReadOnlyList<ICardDataInstanceProvider> _deckCards,
        IPlayerData _playerData)
    {
        shopSystemData = _shopSystemData;
        deckCards = _deckCards;
        playerData = _playerData;
    }

    public void OpenShop()
    {


    }

    public void PlayerSpawned(IPlayerData _playerData)
    {
        playerData = _playerData;
    }



    /////////////// Pannel & Deck
    public void StartCardSelectModefromPannel(ShopBehaviorType _type, int _selectCount, bool _bSelectforcing)
    {
        if (null == cardPannel)
            return;

        CallPannel(true);
        selectSystem.SetSelectMode(_type, _selectCount, _bSelectforcing, cardPannel.SelectBtn);
    }

    private void ActivatePannel(IReadOnlyList<ICardDataInstanceProvider> _inCards)
    {
        if (null == shopPoolingSystem || null == pannelContent || null == cardPannel)
            return;

        int inCount = _inCards.Count;

        if (0 >= inCount)
            return;

        cardPannel.RentCards.Clear();
        foreach (ICardDataInstanceProvider data in _inCards)
        {
            cardPannel.RentCards.Add(RentCard(data, pannelContent.transform, pannelCardScale));
        }
    }

    public void CallPannel(bool bSelectMode = false, bool bSelectBtnHidden = false)
    {
        if (null == cardPannel)
            return;

        cardPannel.CurrPannelType = CurrentPannel.Deck;
        cardPannel.gameObject.SetActive(true);
        cardPannel.SetupSelectMode(bSelectMode, bSelectBtnHidden);

        ActivatePannel(deckCards);
    }

    public void DeactivatePannel()
    {

    }

    [Button]
    private void TestCall_PannelSelectMode()
    {
        StartCardSelectModefromPannel(ShopBehaviorType.Upgrade, 2, true);
    }


    ////////////// PoolingCard

    public ShopCardInstance RentCard(ICardDataInstanceProvider data, Transform attachTransform, Vector3 cardSize)
    {
        var card = shopPoolingSystem?.RentCard();
        card.ApplyData(data);

        card.transform.SetParent(attachTransform);
        card.Motion.SetScale(cardSize);

        // 알아서 Active On
        return card;
    }

    public ShopCardInstance RentCard(ICardDataInstanceProvider data, Vector3 cardSize)
    {
        var card = shopPoolingSystem?.RentCard();
        card.ApplyData(data);
        card.Motion.SetOriginScale(cardSize);

        return card;
    }

    public ShopCardInstance RentCard(ICardDataInstanceProvider data)
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


    ////////////// SelectSystem

    public void ToggleSelect(ShopCardInstance card)
    {
        selectSystem.ToggleSelect(card);
    }

    public bool SelectComplete()
    {
        return selectSystem.SelectComplete();
    }



    ////////////// Click
    ///
    private void OnClick_PickUpCard()
    {
        Debug.Log("[Shop] PickUpCard clicked");

        CardPackRerollEvent?.Invoke();
        selectSystem?.SetSelectMode(ShopBehaviorType.PickUp, pickUpCardCount, pickUpCardForce
            , pickUpSystem.GetPickUpButton(), true);

        pickUpSystem?.PickUpCardMode(shopSystemData.cardMerchandiseData);
    }

    private void OnClick_EnforceCard()
    {
        if (DeletedComplete || EnforcedComplete)
            return;

        Debug.Log("[Shop] EnforceCard clicked");
        StartCardSelectModefromPannel(ShopBehaviorType.Upgrade, enforceCardCount, true);
    }

    private void OnClick_DeleteCard()
    {
        if (DeletedComplete || EnforcedComplete)
            return;

        Debug.Log("[Shop] DeleteCard clicked");
        StartCardSelectModefromPannel(ShopBehaviorType.Delete, deleteCardCount, true);
    }

    private void OnClick_ViewDeck()
    {
        Debug.Log("[Shop] ViewDeck clicked");
    }

    private void OnClick_NextStage()
    {
        Debug.Log("[Shop] NextStage clicked");

        ShopIsClosedEvent?.Invoke();
    }

    public void OutputSelectedCards(List<ICardDataInstanceProvider> cards, ShopBehaviorType type)
    {
        foreach(var c in cards)
        {
            Debug.Log(c.GetCardDataProvider().cardName);
        }

        ShopUIOutputEvent?.Invoke(cards, type);
    }

    // For PickUpCard

    // For EnforceCard

    // For DeleteCard

    // For ViewDeck

    // For NextStage
}
