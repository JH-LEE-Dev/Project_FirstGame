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
    public event Action<int> ShopBillingEvent;

    //외부 의존성
    private IShopSystemData shopSystemData;
    private IPlayerData playerData;

    //현재 게임 시스템의 카드 정보.
    IReadOnlyList<ICardDataInstanceProvider> deckCards;
    List<ICardDataInstanceProvider> possibleList = new(50);

    [Header("Starlight Settings")]
    [SerializeField] private int pickupPrice = 10;
    [SerializeField] private int upgradePrice = 20;
    [SerializeField] private int deletePrice = 20;
    [SerializeField] private string failPayment = "보유 중인 스타라이트가 부족합니다.";

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

    private ShopBehaviorType prevSelectMode = ShopBehaviorType.None;

    [Header("System")]
    private ShopPoolingSystem shopPoolingSystem;
    private ShopSelectSystem selectSystem;

    [Header("Other UI")]
    [SerializeField] private WarningUI warningUI;

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
    private int currentPrice = 0;
    private int pickupPayCnt = 1;

    private void OnEnable()
    {
        
    }

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
        pickupPayCnt = 1;
    }

    public void PlayerSpawned(IPlayerData _playerData)
    {
        playerData = _playerData;
    }

    #region CardPannel & Deck System

    private void CheckUpgradeCardList()
    {
        if (ShopBehaviorType.Upgrade == prevSelectMode)
            return;

        possibleList.Clear();
        for (int i = 0; i < deckCards.Count; ++i)
        {
            if (true == deckCards[i].IsUpgraded() || false == deckCards[i].GetCardDataProvider().bUpgradable)
                continue;

            possibleList.Add(deckCards[i]);
        }
    }

    public bool StartCardSelectModefromPannel(ShopBehaviorType _type, int _selectCount, bool _bSelectforcing)
    {
        if (null == cardPannel)
            return false;

        bool bOpenList = false;

        if (ShopBehaviorType.Upgrade == _type)
        {
            CheckUpgradeCardList();
            bOpenList = CallPannel(true, possibles: possibleList);
        }
        else
            bOpenList = CallPannel(true);

        selectSystem.SetSelectMode(_type, _selectCount, _bSelectforcing, cardPannel.SelectBtn);
        return bOpenList;
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
            ShopCardInstance shopCard = RentCard(data, pannelContent.transform, pannelCardScale);
            if (null == shopCard)
                continue;

            cardPannel.RentCards.Add(shopCard);
        }
    }

    public bool CallPannel(bool bSelectMode = false, bool bSelectBtnHidden = false, List<ICardDataInstanceProvider> possibles = null)
    {
        if (null == cardPannel)
            return false;

        var openList = null != possibles ? possibles : deckCards;
        if (0 >= openList.Count)
            return false;

        cardPannel.CurrPannelType = CurrentPannel.Deck;
        cardPannel.gameObject.SetActive(true);
        cardPannel.SetupSelectMode(bSelectMode, bSelectBtnHidden);

        ActivatePannel(openList);
        return true;
    }

    #endregion

    #region Pooling Card System

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

    #endregion

    #region SelectSystem

    public void ToggleSelect(ShopCardInstance card)
    {
        selectSystem.ToggleSelect(card);
    }

    public bool SelectComplete()
    {
        return selectSystem.SelectComplete();
    }

    #endregion

    #region Click Events
    ///
    private void OnClick_PickUpCard()
    {
        if (!CheckPayment(1, pickupPrice * pickupPayCnt))
        {
            warningUI?.Play(failPayment);
            return;
        }

        CardPackRerollEvent?.Invoke();
        selectSystem?.SetSelectMode(ShopBehaviorType.PickUp, pickUpCardCount, pickUpCardForce
            , pickUpSystem.GetPickUpButton(), true);

        pickUpSystem?.PickUpCardMode(shopSystemData.cardMerchandiseData);

        prevSelectMode = ShopBehaviorType.PickUp;
    }

    private void OnClick_EnforceCard()
    {
        if (DeletedComplete || EnforcedComplete)
            return;

        if (!CheckPayment(1, upgradePrice))
        {
            warningUI?.Play(failPayment);
            return;
        }

        if (!StartCardSelectModefromPannel(ShopBehaviorType.Upgrade, enforceCardCount, true))
        {
            // 강화 카드가 더 이상 존재하지 않을 경우
            warningUI?.Play("더 이상 강화 가능한 카드가 없습니다.");
        }

        prevSelectMode = ShopBehaviorType.Upgrade;
    }

    private void OnClick_DeleteCard()
    {
        if (DeletedComplete || EnforcedComplete)
            return;

        if (!CheckPayment(1, deletePrice))
        {
            warningUI?.Play(failPayment);
            return;
        }

        if (!StartCardSelectModefromPannel(ShopBehaviorType.Delete, deleteCardCount, true))
        {
            // 삭제 카드가 더 이상 존재하지 않을 경우
            warningUI?.Play("더 이상 삭제 가능한 카드가 없습니다.");
        }

        prevSelectMode = ShopBehaviorType.Delete;
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
        if (ShopBehaviorType.Upgrade == prevSelectMode)
            prevSelectMode = ShopBehaviorType.None;

        ShopUIOutputEvent?.Invoke(cards, type);

        if (ShopBehaviorType.PickUp != type)
            Payment(type);
    }

    #endregion

    #region Payment Functions

    private bool CheckPayment(int _haveCoin, int _needCoin) => _needCoin <= _haveCoin;

    public void Payment(ShopBehaviorType type)
    {
        switch(type)
        {
            case ShopBehaviorType.PickUp:
                currentPrice = pickupPrice * pickupPayCnt++;
                break;

            case ShopBehaviorType.Upgrade:
                currentPrice = upgradePrice;
                upgradePrice *= 2;
                break;

            case ShopBehaviorType.Delete:
                currentPrice = deletePrice;
                deletePrice *= 2;
                break;
        }

        ShopBillingEvent.Invoke(0);
    }

    #endregion
}
