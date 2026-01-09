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

    public Action<Vector3, CardDataInstance> DrawEvent;

    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private ClickCatchSystem clickCatchSystem;

    [SerializeField] private HandSystem handSystem;
    public HandSystem HandSystem => handSystem;
    [SerializeField] private DeckSystem deckSystem;
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

    // 덱, 묘지, 소멸 공용
    [Header("Pannel")]
    [SerializeField] private GameObject cardPannel = null;
    [SerializeField] private GameObject pannelContent = null;
    public GameObject PannelContent { get { return pannelContent; } }

    private bool bWorkingBlock = false;
    public bool WorkingBlock { get { return bWorkingBlock; } set { bWorkingBlock = value; } }

    //UIJobQueue
    private List<Job_CardSystemUI> uiJobQueue;

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
        clickCatchSystem?.Init(this);

        BindingFunction();
    }

    private void BindingFunction()
    {
        if (null != handSystem)
        {
            DrawEvent += handSystem.ProcessDraw;
        }
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

        viewCtx?.cardSystemProvider.CardUsed(_card.CardData);
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
    /////////////////




    public void GetDeckCards()
    {
        ActivatePannel(viewCtx.cardSystemProvider.deckCards);
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

    //public void CardDrawed(List<CardDataInstance> cardDataPile)
    //{
    //    if (null == deckSystem)
    //        return;

    //    bWorkingBlock = true;
    //    deckSystem.CardDrawEffect(cardDataPile);
    //    SetText();
    //}

    public void CallDeckPannel(bool _activate)
    {
        cardPannel?.SetActive(_activate);
        GetDeckCards();
    }
    /////////////////////////////////////////////////



    private void SetText()
    {
        deckCntText.text = "Deck : " + viewCtx.cardSystemProvider.deckCards.Count.ToString();
        graveCntText.text = "Grave : " + viewCtx.cardSystemProvider.graveCards.Count.ToString();
        handCntText.text = "Hand : " + viewCtx.cardSystemProvider.handCards.Count.ToString();
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

        viewCtx.cardSystemProvider.CardUsingFinished();

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

    public void RecieveUIJob(List<Job_CardSystemUI> _jobQueue)
    {
        uiJobQueue = _jobQueue;

        SetText();
    }
}
