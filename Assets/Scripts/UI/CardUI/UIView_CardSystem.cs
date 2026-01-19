using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_CardSystem : UIView
{
    public event Action<int> UICommandCompleteEvent;
    public event Action<CardDataInstance> TryCardUseEvent;
    public event Action CardUsingFinishedEvent;

    //사용 승인을 받은 카드
    private MainCardInstance verificationWaitCard;

    //현재 게임 시스템의 카드 정보.
    IReadOnlyList<CardDataInstance> deckCards;
    IReadOnlyList<CardDataInstance> handCards;
    IReadOnlyList<CardDataInstance> graveCards;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [Space]
    [SerializeField] private TMP_Text deckCntText;
    [SerializeField] private TMP_Text graveCntText;
    [SerializeField] private TMP_Text handCntText;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button turnFinishedButton;
    ////////////

    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;

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

    public override void OnDestroy()
    {
        UICommandCompleteEvent = null;
    }

    public void DataInjection(IReadOnlyList<CardDataInstance> _deckCards, IReadOnlyList<CardDataInstance> _handCards,
        IReadOnlyList<CardDataInstance> _graveCards)
    {
        deckCards = _deckCards;
        handCards = _handCards;
        graveCards = _graveCards;
    }

    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this);
        handSystem?.Init(this);
        deckSystem?.Init(this);
        graveSystem?.Init(this);
        extinctionSystem?.Init(this);
    }

    // For PoolingSystem
    public MainCardInstance RentHandCard()
    {
        return poolingSystem?.RentHandCard();
    }
    public void ReturnHandCard(MainCardInstance card)
    {
        poolingSystem?.ReturnHandCard(card);
    }
    /////////////////


    // For HandSystem
    public void TryUseCard(MainCardInstance _card)
    {
        //카드 사용 승인 대기 카드
        verificationWaitCard = _card;

        TryCardUseEvent?.Invoke(_card.CardData);
    }

    public void CardUsingApproved(bool boolean) // true이면 verificationWaitCard -> 사용 승인.
    {
        if (boolean)
        {
            // 우클릭을 했을 때 이쪽으로 온다. (마법카드면 즉시 사용버전)
            handSystem?.UseCard(verificationWaitCard);
            return;

            // 불릿 카드라면?
            int socketIndex = 0; // 넣어져야하는 불릿소켓 인덱스 번호 0~
            Vector3 pos = default; // UIView_Unit.GetSocketPos(socketIndex);
            handSystem?.UseCard(verificationWaitCard, socketIndex, pos);
        }
        else
        {
            //카드 사용 실패.
            Debug.Log("이 카드를 사용할 수 없습니다.");

            verificationWaitCard.Motion.PlayReject();

        }
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

    // state에 맞는 카드들이 묘지로 빨려들어가는 기능
    public void AllCardReturnToPool(CardState state)
    {
        handSystem?.AllCardReturnToPool(state);
    }


    // 불릿 카드 사용했을때, 호출되는 함수
    public void EquipBulletCard(int _index, CardDataInstance _data = null)
    {
        // UIView_Unit.EquipBulletCard(_index, _data);
    }

    // 불릿 카드 뺄 때, 호출되는 함수
    public void UnEquipBulletCard(int _index)
    {
        HandSystem.UnequipBulletToHand(_index);
    }



    /////////////////

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
            if(i < inCount)
            {
                pool[i].ApplyData(_inCards[i]);
                pool[i].transform.SetParent(pannelContent.transform);
                pool[i].gameObject.SetActive(true);
            }
            else
                pool[i].gameObject.SetActive(false);
        }
    }

    // 현재 패 개수 + 지금 들어오는 패에 몇 번째로 들어오는 애인지
    // 

    public Vector2 GetHandTargetEndPos(int currentDrawIdx)
    {
        if (null == handSystem)
            return Vector2.zero;

        int currHandCnt = handSystem.GetCurrentHandCardCount();
        Vector2 NextEndPos = handSystem.PredictRightmostPosForCount(currHandCnt + (currentDrawIdx + 1));

        return NextEndPos;
    }

    public void CallPannel(CurrentPannel _setType)
    {
        if (null == cardPannel)
            return;

        cardPannel.CurrPannelType = _setType;
        cardPannel.gameObject.SetActive(true);

        switch(_setType)
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

    public void CallOneCardDrawed(int currIdx, int _lastIdx, Vector3 _endPos, CardDataInstance _data, GameObject _performer)
    {
        if (currIdx == _lastIdx)
            WorkingBlock = false;

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
        if(null == Rt)
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
    /////////////////////////////////////////////////

    private void SetText()
    {
        deckCntText.text = "Deck : " + deckCards.Count.ToString();
        graveCntText.text = "Grave : " + graveCards.Count.ToString();
        handCntText.text = "Hand : " + handCards.Count.ToString();
    }

    protected override void OnShow()
    {
        base.OnShow();

        SetText();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void RenderUI()
    {

    }

    public void CardUsingFinished()
    {
        handSystem?.CancelPreview();

        turnFinishedButton.gameObject.SetActive(false);
        CardUsingFinishedEvent?.Invoke();
        SetText();
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

    public async void RecieveUIJob(ActionDataBatch_CardSystem _jobBatch)
    {
        var currentActionDataList = _jobBatch.actionDataList;

        float turnWaitSecond = 0.5f;

        int size = currentActionDataList.Count;
        for (int i = 0; i < size; ++i)
        {
            ActionData_CardSystem currentActionData = currentActionDataList[i];

            ActionType_CardSystem currenType = currentActionData.actionDataType;

            switch(currenType)
            {
                case ActionType_CardSystem.PileDraw:

                    DrawingCards(currentActionData.cards);

                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;

                case ActionType_CardSystem.GraveToDeck:
                    graveSystem?.CardMoveToDeckEffect(currentActionDataList[i].cards.Count);

                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;

                case ActionType_CardSystem.AdditionalDraw:

                    DrawingCards(currentActionData.cards);

                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;
                case ActionType_CardSystem.HandToGrave:

                    AllCardReturnToPool(CardState.InHand);

                    // 임시요 : 영우
                    AllCardReturnToPool(CardState.Equipped);
                    await Awaitable.WaitForSecondsAsync(turnWaitSecond);
                    break;

                default: break;
            }
        }

        SetText();

        UICommandCompleteEvent?.Invoke(_jobBatch.idx);
    }

    void DrawingCards(List<CardDataInstance> _datas)
    {
        if (null == deckSystem)
            return;

        bWorkingBlock = true;
        deckSystem.CardDrawEffect(_datas);
    }
}
