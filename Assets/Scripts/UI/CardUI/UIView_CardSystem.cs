using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIView_CardSystem : UIView
{
    //외부 의존성
    private ICardSystemProvider cardSystemProvider;

    //사용 승인을 받은 카드
    private CardInstance verificationWaitCard;

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
    [SerializeField] private ClickCatchSystem clickCatchSystem;

    [SerializeField] private HandSystem handSystem;
    public HandSystem HandSystem => handSystem;
    [SerializeField] private DeckSystem deckSystem;
    public DeckSystem DeckSystem => deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 덱
    [Header("Deck Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();
    [SerializeField] private RectTransform drawEndPoint = null;
    public List<RectTransform> DrawPathPoints { get { return drawPathPoints; } }
    //public RectTransform DrawEndPoint { get { return drawEndPoint; } }

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

    //UIJobQueue
    private List<Job_CardSystemUI> uiJobQueue;

    public void DependencyInjection(ICardSystemProvider _cardSystemProvider)
    {
        cardSystemProvider = _cardSystemProvider;
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
        clickCatchSystem?.Init(this);
    }

    // For PoolingSystem
    public CardInstance RentHandCard()
    {
        return poolingSystem?.RentHandCard();
    }
    public void ReturnHandCard(CardInstance card)
    {
        poolingSystem?.ReturnHandCard(card);
    }
    /////////////////


    // For HandSystem
    public void TryUseCard(CardInstance _card)
    {
        //카드 사용 승인 대기 카드
        verificationWaitCard = _card;

        cardSystemProvider.CardUsed(_card.CardData);
    }

    public void CardUsingApproved(bool boolean) // true이면 verificationWaitCard -> 사용 승인.
    {
        if (boolean)
        {
            // 우클릭을 했을 때 이쪽으로 온다. (즉시 사용)
            handSystem?.UseCard(verificationWaitCard);


        }
        else
        {
            //카드 사용 실패.
            Debug.Log("이 카드를 사용할 수 없습니다.");

            verificationWaitCard.Motion.PlayReject();

        }
    }

    public void OnCardLeftClick(CardInstance _card)
    {
        // 좌클릭을 했을 때 이쪽으로 온다. (프리뷰)
        handSystem?.OnCardLeftClick(_card);
    }

    public void OnCardHoverEnter(CardInstance _card)
    {
        // 호버 ON (벌어지는 연출위함)
        handSystem?.OnCardHoverEnter(_card);
    }
    public void OnCardHoverExit(CardInstance _card)
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
    /////////////////

    public Vector3 GetGraveAnchoredPos()
    {
        if (graveSystem == null) return Vector3.zero;
        return graveSystem.GetComponent<RectTransform>().anchoredPosition;
    }
    public void GetDeckCards()
    {
        ActivatePannel(cardSystemProvider.deckCards);
    }

    public void GetWormholeCards()
    {
        // 추후 구현
    }

    public void GetExtinctionCards()
    {
        // 추후 구현
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

        Camera cam = Camera.main;

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
                ActivatePannel(cardSystemProvider.deckCards); 
                break;

            case CurrentPannel.Grave:
                break;

            case CurrentPannel.Extinction:
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
        poolingSystem?.StarEffects.Release(_performer);
    }

    public void PlayDrawedEffect() => deckSystem?.CardBackDrawedEffect();

    public Vector3 GetDeckWorldPos()
    {
        if (null == deckSystem)
            return Vector3.zero;

        return deckSystem.transform.position;
    }

    public GameObject GetStarPerformerFromPool() => poolingSystem?.StarEffects.Get();
    /////////////////////////////////////////////////

    private void SetText()
    {
        deckCntText.text = "Deck : " + cardSystemProvider.deckCards.Count.ToString();
        graveCntText.text = "Grave : " + cardSystemProvider.graveCards.Count.ToString();
        handCntText.text = "Hand : " + cardSystemProvider.handCards.Count.ToString();
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
        turnFinishedButton.gameObject.SetActive(false);

        cardSystemProvider.CardUsingFinished();

        SetText();

        ClearAllCards();
    }

    public void ClearAllCards()
    {
        //for(int i = 0; i < cards.Count;++i)
        //{
        //    RectTransform card = cards[i].GetComponent<RectTransform>();

        //    card.localPosition = new Vector3(-1000,-1000,card.localPosition.z);
        //}

        //cards.Clear();

        //computeArc();
    }

    public void CardDrawFinished()
    {
        turnFinishedButton.gameObject.SetActive(true);
    }

    public void EnemyTurnStarted()
    {
        //handRoot.gameObject.SetActive(false);
    }

    public void PlayerTurnStarted(int waveIdx)
    {
        //handRoot.gameObject.SetActive(true);
    }

    public async void RecieveUIJob(List<Job_CardSystemUI> _jobQueue)
    {
        Debug.Log(_jobQueue.Count);
        uiJobQueue = _jobQueue;

        // 시작 대기
        //await Awaitable.WaitForSecondsAsync(2f);

        int size = _jobQueue.Count;
        for (int i = 0; i < size; ++i)
        {
            Job_CardSystemUI currentJob = uiJobQueue[i];
            JobType_CardSystemUI currenType = currentJob.jobType;

            switch(currenType)
            {
                case JobType_CardSystemUI.Draw: 
                    DrawedCardsFromTurn(currentJob.cards);
                    await Awaitable.WaitForSecondsAsync(2f);
                    break;

                case JobType_CardSystemUI.GraveToDeck:
                    await Awaitable.WaitForSecondsAsync(2f);
                    break;

                case JobType_CardSystemUI.AdditionalDraw:
                    DrawedCardsFromTurn(currentJob.cards);
                    await Awaitable.WaitForSecondsAsync(2f);
                    break;
                case JobType_CardSystemUI.HandToGrave:

                    AllCardReturnToPool(CardState.InHand);
                    await Awaitable.WaitForSecondsAsync(2f);
                    break;

                default: break;
            }
        }

        SetText();
    }

    void DrawedCardsFromTurn(List<CardDataInstance> _datas)
    {
        if (null == deckSystem)
            return;

        bWorkingBlock = true;
        deckSystem.CardDrawEffect(_datas);
    }
}
