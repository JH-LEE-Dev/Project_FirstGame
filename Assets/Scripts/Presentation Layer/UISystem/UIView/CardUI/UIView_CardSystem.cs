using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIView_CardSystem : UIView
{
    /// <summary>
    /// 시스템 속성 -------------------------------------------------------
    /// </summary>
    //외부 방송 이벤트
    public event Action<int> UICommandCompleteEvent;
    public event Action<ICardDataInstanceProvider> TryCardUseEvent;
    public event Action CardUsingFinishedEvent;
    public event Action<int, ICardDataInstanceProvider> CardEquippedEvent;
    public event Action<List<ICardDataInstanceProvider>, CardSelectionModeData> CardSelectionEndEvent;

    //UI Job Action Binding
    public delegate float UIActionHandler(CardUIActionData cardUIActionData);
    private UIActionHandler[] uiActionHandlers;

    //For CardSelectionMode
    CardSelectionModeData cardSelectionModeData;

    //-------------------------------------End Line--------------------------







    /// <summary>
    /// 구현 속성 ---------------------------------------------------------
    /// </summary>

    //사용 승인을 받은 카드
    private MainCardInstance verificationWaitCard;


    //현재 게임 시스템의 카드 정보.
    IReadOnlyList<ICardDataInstanceProvider> deckCards;
    IReadOnlyList<ICardDataInstanceProvider> handCards;
    IReadOnlyList<ICardDataInstanceProvider> graveCards;
    IReadOnlyList<ICardDataInstanceProvider> extinctionCards;


    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private DimOverlay dimOverlay;
    [Space]
    [Header("Buttons")]
    [SerializeField] private TurnEndButton turnFinishedButton;
    ////////////


    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;

    [SerializeField] private PathSystem pathSystem;
    public PathSystem PathSystem => pathSystem;

    [SerializeField] private HandSystem handSystem;
    public HandSystem HandSystem => handSystem;
    [SerializeField] private BattleDeckSystem deckSystem;
    public BattleDeckSystem DeckSystem => deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 묘지
    [Header("Graveyard Settings")]
    [SerializeField] private GraveyardSystem graveSystem = null;

    // 소멸
    [Header("Extinction Settings")]
    [SerializeField] private ExtinctionSystem extinctionSystem = null;

    // 덱, 묘지, 소멸 공용
    [Header("Pannel")]
    [SerializeField] private BattleCardPannel cardPannel = null;
    [SerializeField] private GameObject pannelContent = null;
    public GameObject PannelContent { get { return pannelContent; } }
    public BattleCardPannel CardPannel { get { return cardPannel; } }

    // 드로우 중 작업 중지
    private bool bWorkingBlock = false;
    public bool WorkingBlock { get { return bWorkingBlock; } set { bWorkingBlock = value; } }

    //-------------------------------------End Line-----------------------------------





    /// <summary>
    /// 시스템 함수들 ----------------------------------------------------------------
    /// </summary>
    public override void Initialize(UIViewContext ctx)
    {
        base.Initialize(ctx);

        BindUIActionHandlers();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }


    private void BindUIActionHandlers()
    {
        uiActionHandlers = new UIActionHandler[(int)CardUIActionType.MAX];

        void Bind(CardUIActionType type, UIActionHandler handler)
            => uiActionHandlers[(int)type] = handler;

        Bind(CardUIActionType.PileDraw, CardPileDraw);
        Bind(CardUIActionType.GraveCardsToDeck, GraveCardsToDeck);
        Bind(CardUIActionType.ExtinctionCardsToDeck, ExtinctionCardsToDeck);
        Bind(CardUIActionType.CardsToExtinction, CardsToExtinction);
        Bind(CardUIActionType.GraveCardsToHand, GraveCardsToHand);
        Bind(CardUIActionType.CardsToGrave, CardsToGrave);
        Bind(CardUIActionType.AdditionalDraw, CardAdditionalDraw);
        Bind(CardUIActionType.HandCardsToGrave, HandCardsToGrave);
        Bind(CardUIActionType.CardsToHand, CardsToHand);
        Bind(CardUIActionType.CardsToDeck, CardsToDeck);
        Bind(CardUIActionType.CardsValueModified, CardValuesModified);
        Bind(CardUIActionType.CardsUpgraded, CardsUpgraded);
    }
    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection(IReadOnlyList<ICardDataInstanceProvider> _deckCards, IReadOnlyList<ICardDataInstanceProvider> _handCards,
        IReadOnlyList<ICardDataInstanceProvider> _graveCards,IReadOnlyList<ICardDataInstanceProvider> _extinctionCards)
    {
        deckCards = _deckCards;
        handCards = _handCards;
        graveCards = _graveCards;
        extinctionCards = _extinctionCards;

        deckSystem?.SetupCount(CountUIType.VisibleWhenZero, deckCards.Count);
        graveSystem?.SetupCount(CountUIType.VisibleWhenZero, graveCards.Count);
        extinctionSystem?.SetupCount(CountUIType.VisibleWhenZero, extinctionCards.Count);

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.OnCompleteAction(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this, viewCtx.cardLocalizationSystem);
        handSystem?.Init(this);
        deckSystem?.Init(this);
        graveSystem?.Init(this);
        extinctionSystem?.Init(this);
        cardPannel?.Init(this);
        //pathSystem?.Init(this);
    }

    public override void OnDestroy()
    {
        UICommandCompleteEvent = null;
    }

    //----------------------------------End Line-----------------------------------------------------








    /// <summary>
    /// 외부 신호 반응 함수들 ------------------------------------------------------------
    /// </summary>
    public void CardUsingFinished()
    {
        handSystem?.CancelPreview();

        turnFinishedButton.gameObject.SetActive(false);
        CardUsingFinishedEvent?.Invoke();
    }

    public void CardUsePhaseStarted()
    {
        turnFinishedButton.gameObject.SetActive(true);
    }

    public void EnemyTurnStarted()
    {
        //handRoot.gameObject.SetActive(false);
    }

    ///////////////////////////////////
    public void GameStarted()
    {
        UpdateCardsCount();
    }

    public void WaveStarted()
    {
        UpdateCardsCount();
    }

    public void PlayerTurnStarted()
    {
        //handRoot.gameObject.SetActive(true);
    }

    public async void ReceiveUIAction(CardUIActionBatch _actionBatch)
    {
        var currentActionDataList = _actionBatch.actionList;

        int size = currentActionDataList.Count;

        UpdateCardsCount();

        for (int i = 0; i < size; ++i)
        {
            CardUIActionData currentActionData = currentActionDataList[i];

            CardUIActionType currentType = currentActionData.uiActionType;

            float turnWaitTime = uiActionHandlers[(int)currentType].Invoke(currentActionData);

            await Awaitable.WaitForSecondsAsync(turnWaitTime);
        }

        UICommandCompleteEvent?.Invoke(_actionBatch.idx);
    }

    public void CardUsingApproved(bool boolean, int slotIdx, Transform slotTransform) // true이면 verificationWaitCard -> 사용 승인.
    {
        if (boolean)
        {
            handSystem?.UseCard(verificationWaitCard, slotIdx, slotTransform);
        }
        else
        {
            //카드 사용 실패.
            Debug.Log("이 카드를 사용할 수 없습니다.");

            verificationWaitCard.Motion.PlayReject();
        }
    }

    public void ResetCardUISystem()
    {
        AllDeActivatePannel();
    }

    //----------------------------------End Line-----------------------------------------------------






    /// <summary>
    /// UI Job 함수들 ------------------------------------------------------
    /// </summary>
    private float CardPileDraw(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        Debug.Log("카드 뭉치가 드로우됨.");

        DrawingCards(uiActionData.cards);

        return turnWaitTime;
    }

    private float CardAdditionalDraw(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        Debug.Log("카드가 추가로 드로우됨.");

        DrawingCards(uiActionData.cards);

        return turnWaitTime;
    }


    private float GraveCardsToDeck(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        Debug.Log("묘지의 카드가 덱으로 감");

        graveSystem?.CardMoveToDeckEffect(uiActionData.cards.Count);

        return turnWaitTime;
    }

    private float HandCardsToGrave(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        Debug.Log("패의 카드가 묘지로 감");

        ReturnStateAllCard(CardState.InHand, CardReturnType.FlyToGrave);

        return turnWaitTime;
    }

    private float CardsToExtinction(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        if (uiActionData.cardSystemContextType == GameSystemActionContextType.UsedCardsToExtinction)
        {
            Debug.Log("사용된 카드가 소멸로 감.");
            ReturnCard(uiActionData.cards, CardReturnType.Extinction);

        }
        else if (uiActionData.cardSystemContextType == GameSystemActionContextType.SlotCardsToExtinction)
        {
            Debug.Log("슬롯에 있던 카드가 소멸로 감.");
            ReturnStateAllCard(CardState.Equipped);
        }

        return turnWaitTime;
    }

    private float CardsToGrave(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        if (uiActionData.cardSystemContextType == GameSystemActionContextType.UsedCardsToGrave)
        {
            Debug.Log("사용된 카드가 묘지로 감.");
            ReturnCard(uiActionData.cards, CardReturnType.MagicUse);

        }
        else if (uiActionData.cardSystemContextType == GameSystemActionContextType.SlotCardsToGrave)
        {
            Debug.Log("슬롯에 있던 카드가 묘지로 감.");
            ReturnStateAllCard(CardState.Equipped);
        }

        return turnWaitTime;
    }

    private float ExtinctionCardsToDeck(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        Debug.Log("묘지의 카드가 패로 감");

        return turnWaitTime;
    }

    private float GraveCardsToHand(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        Debug.Log("묘지의 카드가 패로 감");

        graveSystem?.CardDrawToHands(uiActionData.cards);

        return turnWaitTime;
    }

    private float CardsToHand(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        if (uiActionData.cardSystemContextType == GameSystemActionContextType.DuplicateCardCardsToHand)
        {
            Debug.Log("복사된 카드가 패로 감");
            // 복사된 카드가 패로 들어옴. 임시.
            foreach (var card in uiActionData.cards)
            {
                MakeCardInHand(card);
            }
        }

        return turnWaitTime;
    }

    private float CardsToDeck(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        if (uiActionData.cardSystemContextType == GameSystemActionContextType.DuplicateCardCardsToDeck)
        {
            Debug.Log("복사된 카드가 덱으로 감");
            //복사된 카드가 덱으로 들어옴.
        }

        if (uiActionData.cardSystemContextType == GameSystemActionContextType.HandCardsToDeck)
        {
            ReturnCard(uiActionData.cards, CardReturnType.Extinction);
        }

        return turnWaitTime;
    }

    private float CardValuesModified(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        //카드 수치가 증폭됨.
        Debug.Log("카드 수치가 증폭됨");

        return turnWaitTime;
    }

    private float CardsUpgraded(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0f;

        if (uiActionData.cardSystemContextType == GameSystemActionContextType.UpgradeCardsFromHand)
        {
            Debug.Log("패에 있는 카드가 강화됨");
            handSystem.UpgradeCard(uiActionData.cards);
        }

        return turnWaitTime;
    }

    //----------------------------------End Line-----------------------------------------------------

    #region Pooling System

    public MainCardInstance RentHandCard()
    {
        return poolingSystem?.RentHandCard();
    }

    public void ReturnHandCard(MainCardInstance card)
    {
        poolingSystem?.ReturnHandCard(card);
    }

    public void PlayMagicCardEffect(Vector3 worldPos, float scaleMul, System.Action onComplete = null)
    {
        poolingSystem?.PlayMagicCardEffect(worldPos, scaleMul, onComplete);
    }

    #endregion

    #region Hand System

    public void TryUseCard(MainCardInstance _card)
    {
        //카드 사용 승인 대기 카드
        verificationWaitCard = _card;

        TryCardUseEvent?.Invoke(_card.CardData);
    }

    public void OnCardLeftClick(MainCardInstance _card)
    {
        // 좌클릭을 했을 때 이쪽으로 온다. (프리뷰)
        handSystem?.OnCardLeftClick(_card);
    }

    public void OnCardHoverEnter(MainCardInstance _card)
    {
        // 호버 ON (벌어지는 연출위함)
        handSystem?.OnCardHoverEnter(_card);
    }
    public void OnCardHoverExit(MainCardInstance _card)
    {
        // 호버 OFF (축소되는 연출 위함)
        handSystem?.OnCardHoverExit(_card);
    }

    public void CancelPreview()
    {
        handSystem?.CancelPreview();
    }

    // 불릿 카드 사용했을때, 호출되는 함수
    public void EquipBulletCard(int _index, ICardDataInstanceProvider _data = null)
    {
        CardEquippedEvent?.Invoke(_index, _data);
    }

    // 불릿 카드 뺄 때, 호출되는 함수
    public void UnEquipBulletCard(int _index)
    {
        handSystem.UnequipBulletToHand(_index);
    }

    // 옵션 등으로 강제로 카드를 추가하는 함수
    public void MakeCardInHand(ICardDataInstanceProvider _cardData)
    {
        int cnt = handSystem.GetCurrentHandCardCount();

        Vector2 handPos = handSystem.PredictRightmostPosForCount(cnt);

        handSystem.ProcessDraw(handPos, _cardData);
    }

    public void StartCardSelectMode(CardSelectionModeData _data, int _selectCount, bool _bSelectforcing)
    {
        cardSelectionModeData = _data;
        if (_data.selectCardPileType == SelectCardPileType.Hand)
        {
            // _selectCount은 선택 개수
            // _bSelectforcing은 반드시 _selectCount만큼 선택해야 하는가?
            handSystem.StartCardSelectMode(_data, _selectCount, _bSelectforcing);
            dimOverlay.SetDimOverlayActive(true);
        }
        else if(_data.selectCardPileType == SelectCardPileType.Grave)
        {
            StartCardSelectModefromPannel(CurrentPannel.Grave, _selectCount, _bSelectforcing);
        }
        else if (_data.selectCardPileType == SelectCardPileType.Extinction)
        {
            StartCardSelectModefromPannel(CurrentPannel.Extinction, _selectCount, _bSelectforcing);
        }
        else if (_data.selectCardPileType == SelectCardPileType.Deck)
        {
            StartCardSelectModefromPannel(CurrentPannel.Deck, _selectCount, _bSelectforcing);
        }
    }

    public void EndCardSelectMode(List<ICardDataInstanceProvider> _cards)
    {
        dimOverlay.SetDimOverlayActive(false);

        if (cardSelectionModeData.selectionMode == CardSelectionMode.DuplicateCardsToHand)
            ReturnCard(_cards, CardReturnType.StayHand);
        else if(cardSelectionModeData.selectionMode == CardSelectionMode.DuplicateCardsToDeck)
            ReturnCard(_cards, CardReturnType.StayHand);
        else if(cardSelectionModeData.selectionMode == CardSelectionMode.UpgradeCardsToHand)
            ReturnCard(_cards, CardReturnType.StayHand);

        CardSelectionEndEvent?.Invoke(_cards, cardSelectionModeData);
    }

    public bool GetChooseMode() { return handSystem.GetChooseMode(); }

    // 현재 패 개수 + 지금 들어오는 패에 몇 번째로 들어오는 애인지
    public Vector2 GetHandTargetEndPos(int currentDrawIdx)
    {
        if (null == handSystem)
            return Vector2.zero;

        int currHandCnt = handSystem.GetCurrentHandCardCount();

        Vector2 NextEndPos = handSystem.PredictRightmostPosForCount(currHandCnt + (currentDrawIdx + 1));

        return NextEndPos;
    }

    // CardState : 현재 어디에있는지. (패, 프리뷰 등)
    public void ReturnStateAllCard(CardState state, CardReturnType type = CardReturnType.Temp, float delay = 0f, float interval = 0.09f)
    {
        if (state == CardState.Equipped)
        {
            handSystem?.ReturnStateAllCard(state, CardReturnType.Temp);
        }
        else handSystem?.ReturnStateAllCard(state, type, delay, interval);
    }


    public void ReturnCard(List<ICardDataInstanceProvider> cardDataList, CardReturnType type = CardReturnType.Temp, float delay = 0f)
    {
        handSystem.ReturnCard(cardDataList, type, delay);
    }

    #endregion

    #region CardPannel System

    private void ActivatePannel(IReadOnlyList<ICardDataInstanceProvider> _inCards)
    {
        if (null == poolingSystem || null == pannelContent)
            return;

        var pool = poolingSystem.OtherCardPool;

        int inCount = _inCards.Count;
        int poolCount = pool.Count;

        if (0 >= poolCount || inCount > poolCount)
            return;

        for (int i = 0; i < poolCount; ++i)
        {
            if (i < inCount)
            {
                pool[i].ApplyData(_inCards[i]);
                pool[i].transform.SetParent(pannelContent.transform);
                pool[i].gameObject.SetActive(true);
            }
            else
                pool[i].gameObject.SetActive(false);
        }
    }

    public void CallPannel(CurrentPannel _setType, bool bSelectMode = false)
    {
        if (null == cardPannel)
            return;

        cardPannel.CurrPannelType = _setType;
        cardPannel.gameObject.SetActive(true);
        cardPannel.SetupSelectMode(bSelectMode);

        switch (_setType)
        {
            case CurrentPannel.Deck:
                ActivatePannel(deckCards);
                break;

            case CurrentPannel.Grave:
                ActivatePannel(graveCards);
                break;

            case CurrentPannel.Extinction:
                ActivatePannel(extinctionCards);
                break;
        }
    }

    public void CallPannel(CurrentPannel _setType, bool bSelectMode, IReadOnlyList<ICardDataInstanceProvider> openList)
    {
        if (null == cardPannel || 0 >= openList.Count)
            return;

        cardPannel.CurrPannelType = _setType;
        cardPannel.gameObject.SetActive(true);
        cardPannel.SetupSelectMode(bSelectMode);

        ActivatePannel(openList);
    }

    public void ForceDeActivatePannelSelf(CurrentPannel callType)
    {
        if (null == cardPannel || callType != cardPannel.CurrPannelType)
            return;

        cardPannel.gameObject.SetActive(false);
    }

    public void AllDeActivatePannel()
    {
        ForceDeActivatePannelSelf(CurrentPannel.Grave);
        ForceDeActivatePannelSelf(CurrentPannel.Deck);
        ForceDeActivatePannelSelf(CurrentPannel.Extinction);
    }
    public void StartCardSelectModefromPannel(CurrentPannel _pannelType, int _selectCount, bool _bSelectforcing)
    {
        if (null == cardSelectionModeData.availableCards)
            return;

        CallPannel(_pannelType, true, cardSelectionModeData.availableCards);
        cardPannel?.StartSelectMode(_selectCount, _bSelectforcing);
    }

    public void EndCardSelectModefromPannel(List<ICardDataInstanceProvider> _cards)
    {
        CardSelectionEndEvent?.Invoke(_cards, cardSelectionModeData);
        UpdateCardsCount();
    }

    #endregion

    #region Graveyard System
    /////////////////////////////////// For GraveSystem
    public Vector3 GetGraveAnchoredPos()
    {
        if (graveSystem == null) return Vector3.zero;
        return graveSystem.GetComponent<RectTransform>().anchoredPosition;
    }

    public Vector3 GetGravePos()
    {
        if (graveSystem == null) return Vector3.zero;
        return graveSystem.transform.position;
    }

    public void CallOneCardDrawedBlock(int currIdx, int _lastIdx, Vector3 _endPos, ICardDataInstanceProvider _data, GameObject _performer)
    {
        if (currIdx == _lastIdx)
            WorkingBlock = false;

        handSystem?.ProcessDraw(_endPos, _data);
        poolingSystem?.StarEffects?.Release(_performer);
    }

    public void CallOneCardDrawed(Vector3 _endPos, ICardDataInstanceProvider _data, GameObject _performer)
    {
        handSystem?.ProcessDraw(_endPos, _data);
        poolingSystem?.StarEffects?.Release(_performer);
    }

    public void CallGraveToDeckFinished(int currIdx, GameObject _performer)
    {
        // 풀링한테 지워달라고 요청
        poolingSystem?.StarEffects?.Release(_performer);
        // 덱 받은 모션 재생
        deckSystem?.InDeckFromGraveMotion();

        // 현재 받은 인덱스가 마지막 주자 인덱스랑 같으면 마무리 모션
        graveSystem?.MoveToDeckFinishMotion(currIdx);
    }

    public void PlayDrawedEffect() => deckSystem?.CardBackDrawedEffect();
    public void PlayMoveToDeckMotion() => graveSystem?.CardMoveToDeckMotion();

    #endregion

    #region Effect 

    public void SpawnStarAtoB(bool bCardSpawn, int _idx, Vector3 _startWorldPos, Vector3 _targetWorldPos, ICardDataInstanceProvider _data = null)
    {
        GameObject star = GetStarPerformerFromPool(_startWorldPos);
        VFX_CardStar vfx = star?.GetComponent<VFX_CardStar>();
        if (null == vfx)
            return;

        Vector3[] path = pathSystem.GetDragPath(star, _startWorldPos, _targetWorldPos, 150f, DragDir.UP);

        vfx.CardDataInstance = _data;

        float delay = 0.15f;
        float duration = 0.35f;
        Ease ease = Ease.OutQuad;

        if (bCardSpawn)
            vfx.PlayCardSpawnEvent(_idx, delay, duration, ease, path, SpawnCardStarEvent, SpawnCardCompleteEvent);
        else
            vfx.PlayCardSpawnEvent(_idx, delay, duration, ease, path, NotCardSpawnStarEvent, NotCardSpawnCompleteEvent);
    }

    private void SpawnCardStarEvent(VFX_CardStar vfx)
    {

    }

    private void SpawnCardCompleteEvent(VFX_CardStar vfx)
    {
        CallOneCardDrawed(vfx.TargetPos, vfx.CardDataInstance, vfx.gameObject);
    }

    private void NotCardSpawnStarEvent(VFX_CardStar vfx)
    {

    }

    private void NotCardSpawnCompleteEvent(VFX_CardStar vfx)
    {
        poolingSystem?.StarEffects?.Release(vfx.gameObject);
    }

    public GameObject GetStarPerformerFromPool(Transform target)
    {
        GameObject getObj = poolingSystem?.StarEffects.Get();

        if (null != getObj)
            getObj.transform.position = target.position;

        return getObj;
    }

    public GameObject GetStarPerformerFromPool(Vector3 target)
    {
        GameObject getObj = poolingSystem?.StarEffects.Get();

        if (null != getObj)
            getObj.transform.position = target;

        return getObj;
    }
    #endregion

    #region Counting System

    private void UpdateCardsCount()
    {
        graveSystem?.SetCount(graveCards.Count);
        deckSystem?.SetCount(deckCards.Count);
        extinctionSystem?.SetCount(extinctionCards.Count);
    }

    #endregion 

    private void DrawingCards(List<ICardDataInstanceProvider> _datas)
    {
        if (null == deckSystem || 0 >= _datas.Count)
            return;

        bWorkingBlock = true;
        deckSystem.CardDrawEffect(_datas);
    }

    [Button]
    private void TestCall_PannelSelectMode()
    {
        StartCardSelectModefromPannel(CurrentPannel.Grave, 3, true);
    }
}
