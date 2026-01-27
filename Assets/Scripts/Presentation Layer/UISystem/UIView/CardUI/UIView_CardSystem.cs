using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using DG.Tweening;

public class UIView_CardSystem : UIView
{
    /// <summary>
    /// 시스템 속성 -------------------------------------------------------
    /// </summary>
    //외부 방송 이벤트
    public event Action<int> UICommandCompleteEvent;
    public event Action<CardDataInstance> TryCardUseEvent;
    public event Action CardUsingFinishedEvent;
    public event Action<int, CardDataInstance> CardEquippedEvent;
    public event Action<List<CardDataInstance>, CardSelectionModeData> CardSelectionEndEvent;


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
    IReadOnlyList<CardDataInstance> deckCards;
    IReadOnlyList<CardDataInstance> handCards;
    IReadOnlyList<CardDataInstance> graveCards;


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
    [SerializeField] private DeckSystem deckSystem;
    public DeckSystem DeckSystem => deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 묘지
    [Header("Graveyard Settings")]
    [SerializeField] private GraveyardSystem graveSystem = null;

    // 소멸
    [Header("Extinction Settings")]
    [SerializeField] private ExtinctionSystem extinctionSystem = null;

    // 덱, 묘지, 소멸 공용
    [Header("Pannel")]
    [SerializeField] private CardPannel cardPannel = null;
    [SerializeField] private GameObject pannelContent = null;
    public GameObject PannelContent { get { return pannelContent; } }

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

        uiActionHandlers[(int)CardUIActionType.PileDraw] = (uiActionData) => CardPileDraw(uiActionData);
        uiActionHandlers[(int)CardUIActionType.GraveCardsToDeck] = (uiActionData) => GraveCardsToDeck(uiActionData);
        uiActionHandlers[(int)CardUIActionType.ExtinctionCardsToDeck] = (uiActionData) => ExtinctionCardsToDeck(uiActionData);
        uiActionHandlers[(int)CardUIActionType.CardsToExtinction] = (uiActionData) => CardsToExtinction(uiActionData);
        uiActionHandlers[(int)CardUIActionType.GraveCardsToHand] = (uiActionData) => GraveCardsToHand(uiActionData);
        uiActionHandlers[(int)CardUIActionType.CardsToGrave] = (uiActionData) => CardsToGrave(uiActionData);
        uiActionHandlers[(int)CardUIActionType.AdditionalDraw] = (uiActionData) => CardAdditionalDraw(uiActionData);
        uiActionHandlers[(int)CardUIActionType.HandCardsToGrave] = (uiActionData) => HandCardsToGrave(uiActionData);
        uiActionHandlers[(int)CardUIActionType.CardsToHand] = (uiActionData) => CardsToHand(uiActionData);
        uiActionHandlers[(int)CardUIActionType.CardsToDeck] = (uiActionData) => CardsToDeck(uiActionData);
    }

    public void DataInjection(IReadOnlyList<CardDataInstance> _deckCards, IReadOnlyList<CardDataInstance> _handCards,
        IReadOnlyList<CardDataInstance> _graveCards)
    {
        deckCards = _deckCards;
        handCards = _handCards;
        graveCards = _graveCards;

        deckSystem?.SetupCount(CountUIType.VisibleWhenZero, deckCards.Count);
        graveSystem?.SetupCount(CountUIType.VisibleWhenZero, graveCards.Count);
        //extinctionSystem?.SetupCount(CountUIType.VisibleWhenZero, extinctionCards.Count);
    }

    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.OnCompleteAction(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this);
        handSystem?.Init(this);
        deckSystem?.Init(this);
        graveSystem?.Init(this);
        extinctionSystem?.Init(this);
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

    public void CardDrawFinished()
    {
        turnFinishedButton.gameObject.SetActive(true);
    }

    public void EnemyTurnStarted()
    {
        //handRoot.gameObject.SetActive(false);
    }

    public void PlayerTurnStarted()
    {
        //handRoot.gameObject.SetActive(true);
    }

    public async void ReceiveUIAction(CardUIActionBatch _actionBatch)
    {
        var currentActionDataList = _actionBatch.actionList;

        int size = currentActionDataList.Count;
        for (int i = 0; i < size; ++i)
        {
            CardUIActionData currentActionData = currentActionDataList[i];

            CardUIActionType currentType = currentActionData.uiActionType;

            float turnWaitTime = uiActionHandlers[(int)currentType].Invoke(currentActionData);

            await Awaitable.WaitForSecondsAsync(turnWaitTime);
        }

        UpdateCardsCounts();

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

    //----------------------------------End Line-----------------------------------------------------






    /// <summary>
    /// UI Job 함수들 ------------------------------------------------------
    /// </summary>
    private float CardPileDraw(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        DrawingCards(uiActionData.cards);

        return turnWaitTime;
    }

    private float CardAdditionalDraw(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        DrawingCards(uiActionData.cards);

        return turnWaitTime;
    }


    private float GraveCardsToDeck(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        graveSystem?.CardMoveToDeckEffect(uiActionData.cards.Count);

        return turnWaitTime;
    }

    private float HandCardsToGrave(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        ReturnStateAllCard(CardState.InHand, CardReturnType.FlyToGrave);

        return turnWaitTime;
    }

    private float CardsToExtinction(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        if (uiActionData.cardSystemContextType == CardSystemContextType.UsedCardsToExtinction)
            Debug.Log("사용된 카드가 소멸로 감.");
        else if (uiActionData.cardSystemContextType == CardSystemContextType.SlotCardsToExtinction)
        {
            Debug.Log("슬롯에 있던 카드가 소멸로 감.");
            ReturnStateAllCard(CardState.Equipped);
        }

        ReturnCard(uiActionData.cards, CardReturnType.Extinction);

        return turnWaitTime;
    }

    private float CardsToGrave(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        if (uiActionData.cardSystemContextType == CardSystemContextType.UsedCardsToGrave)
            Debug.Log("사용된 카드가 묘지로 감.");
        else if (uiActionData.cardSystemContextType == CardSystemContextType.SlotCardsToGrave)
        {
            Debug.Log("슬롯에 있던 카드가 묘지로 감.");
            ReturnStateAllCard(CardState.Equipped);
        }

        ReturnCard(uiActionData.cards, CardReturnType.FlyToGrave);

        return turnWaitTime;
    }

    private float ExtinctionCardsToDeck(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        return turnWaitTime;
    }

    private float GraveCardsToHand(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        graveSystem?.CardDrawToHands(uiActionData.cards);

        return turnWaitTime;
    }

    private float CardsToHand(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        if (uiActionData.cardSystemContextType == CardSystemContextType.DuplicateCardCardsToHand)
        {
            Debug.Log("복사된 카드가 패로 감");
            // 복사된 카드가 패로 들어옴.
        }

        return turnWaitTime;
    }

    private float CardsToDeck(CardUIActionData uiActionData)
    {
        //설정할 것.
        float turnWaitTime = 0.5f;

        if (uiActionData.cardSystemContextType == CardSystemContextType.DuplicateCardCardsToDeck)
        {
            Debug.Log("복사된 카드가 덱으로 감");
            //복사된 카드가 덱으로 들어옴.
        }

        return turnWaitTime;
    }

    //----------------------------------End Line-----------------------------------------------------






    /// <summary>
    /// UI 구현 함수들 ------------------------------------------------------------------------------
    /// </summary>
    /////////////////////////////////// For PoolingSystem
    public MainCardInstance RentHandCard()
    {
        return poolingSystem?.RentHandCard();
    }
    public void ReturnHandCard(MainCardInstance card)
    {
        poolingSystem?.ReturnHandCard(card);
    }
    ///////////////////////////////////



    /////////////////////////////////// For HandSystem
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
    public void EquipBulletCard(int _index, CardDataInstance _data = null)
    {
        CardEquippedEvent?.Invoke(_index, _data);
    }

    // 불릿 카드 뺄 때, 호출되는 함수
    public void UnEquipBulletCard(int _index)
    {
        handSystem.UnequipBulletToHand(_index);
    }

    [Button]
    public void SelectModeON()
    {
        StartCardSelectMode(default, 3, true);
    }

    public void StartCardSelectMode(CardSelectionModeData _data, int _selectCount, bool _bSelectforcing)
    {
        cardSelectionModeData = _data;

        // _selectCount은 선택 개수
        // _bSelectforcing은 반드시 _selectCount만큼 선택해야 하는가?
        handSystem.StartCardSelectMode(_selectCount, _bSelectforcing);
        dimOverlay.SetDimOverlayActive(true);
    }

    public void EndCardSelectMode(List<CardDataInstance> _cards)
    {
        dimOverlay.SetDimOverlayActive(false);

        if (cardSelectionModeData.selectionMode == CardSelectionMode.DuplicateToHand)
            ReturnCard(_cards, CardReturnType.FlyToGrave);
        else if(cardSelectionModeData.selectionMode == CardSelectionMode.DuplicateToDeck)
            ReturnCard(_cards, CardReturnType.FlyToGrave);

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
            /* UIView_Unit.UnEquipBulletCardForShoot */
            handSystem?.ReturnStateAllCard(state, CardReturnType.Temp);
        }
        else handSystem?.ReturnStateAllCard(state, type, delay, interval);
    }


    public void ReturnCard(List<CardDataInstance> cardDataList, CardReturnType type = CardReturnType.Temp, float delay = 0f)
    {
        handSystem.ReturnCard(cardDataList, type, delay);
    }

    ///////////////////////////////////



    /////////////////////////////////// For GraveSystem
    public Vector3 GetGraveAnchoredPos()
    {
        if (graveSystem == null) return Vector3.zero;
        return graveSystem.GetComponent<RectTransform>().anchoredPosition;
    }

    private void ActivatePannel(IReadOnlyList<CardDataInstance> _inCards)
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

    public void CallPannel(CurrentPannel _setType)
    {
        if (null == cardPannel)
            return;

        cardPannel.CurrPannelType = _setType;
        cardPannel.gameObject.SetActive(true);

        switch (_setType)
        {
            case CurrentPannel.Deck:
                ActivatePannel(deckCards);
                break;

            case CurrentPannel.Grave:
                ActivatePannel(graveCards);
                break;

            case CurrentPannel.Extinction:
                //ActivatePannel(cards);
                break;
        }
    }

    public void ForceDeActivatePannelSelf(CurrentPannel callType)
    {
        if (null == cardPannel || callType != cardPannel.CurrPannelType)
            return;

        cardPannel.gameObject.SetActive(false);
    }

    public void CallOneCardDrawedBlock(int currIdx, int _lastIdx, Vector3 _endPos, CardDataInstance _data, GameObject _performer)
    {
        if (currIdx == _lastIdx)
            WorkingBlock = false;

        handSystem?.ProcessDraw(_endPos, _data);
        poolingSystem?.StarEffects?.Release(_performer);
    }

    public void CallOneCardDrawed(Vector3 _endPos, CardDataInstance _data, GameObject _performer)
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

    public void SpawnCardStarAtoB(int _idx, Vector3 _startWorldPos, Vector3 _targetWorldPos, CardDataInstance _data = null)
    {
        GameObject star = GetStarPerformerFromPool(_startWorldPos);
        VFX_CardStar vfx = star?.GetComponent<VFX_CardStar>();
        if (null == vfx)
            return;

        Action StartEvent = () =>
        {

        };

        Action CompleteEvent = () =>
        {
            CallOneCardDrawed(_targetWorldPos, _data, star);
        };

        Vector3[] path = pathSystem.GetDragPath(star, _startWorldPos, _targetWorldPos, 150f);

        vfx.CardDataInstance = _data;
        // _idx는 현재 몇번 째 스폰인지 ( for 문에서 여러 개를 소환 할 때 i를 박아 놓으면 알아서 처리 됨, 단일 객체의 경우 0으로 하면 됨 )
        // 0.15 > 스폰 딜레이 ( 내부에서 idx 랑 곱해서 알아서 결정 됨 )
        // 0.35 > duration
        // path는 경로 시스템에서 시작점과 끝점이 있으면 베지어 곡선으로 알아서 만듦
        // 나머지는 위에서 액션 바인딩
        vfx.PlayCardSpawnEvent(_idx, 0.15f, 0.35f, Ease.OutQuad, path, StartEvent, CompleteEvent);
    }

    public Vector3 GetDeckWorldPos()
    {
        if (null == deckSystem)
            return Vector3.zero;

        return deckSystem.transform.position;
    }

    public Vector2 GetDeckAnchoredPos()
    {
        if (null == deckSystem)
            return Vector2.zero;

        RectTransform Rt = deckSystem.GetComponent<RectTransform>();
        if (null == Rt)
            return Vector2.zero;

        return Rt.anchoredPosition;
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
    /////////////////////////////////////////////////

    private void UpdateCardsCounts()
    {
        graveSystem?.SetCount(graveCards.Count);
        deckSystem?.SetCount(deckCards.Count);
    }

    private void DrawingCards(List<CardDataInstance> _datas)
    {
        if (null == deckSystem || 0 >= _datas.Count)
            return;

        bWorkingBlock = true;
        deckSystem.CardDrawEffect(_datas);
    }
}
