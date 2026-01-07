using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIView_CardSystem : UIView
{
    // 신경 쓰지 말기
    public event Action TurnFinishedEvent;

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

    public Action<Vector3, CardDataInstance> drawEvent;

    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private HandSystem handSystem;
    public HandSystem HandSystem => handSystem;
    [SerializeField] private DeckSystem deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 덱
    [Header("Deck Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();
    [SerializeField] private RectTransform drawEndPoint = null;
    public List<RectTransform> DrawPathPoints { get { return drawPathPoints; } }
    public RectTransform DrawEndPoint { get { return drawEndPoint; } }

    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this);
        handSystem?.Init(this);
        deckSystem?.Init(this);

        BindingFunction();
    }

    private void BindingFunction()
    {
        if(null != handSystem)
        {
            Debug.Log("호출");

            drawEvent += handSystem.ProcessDraw;
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
        if (viewCtx.cardSystemProvider.CardUsed(_card.CardData) == false)
            return;

        // 우클릭을 했을 때 이쪽으로 온다. (즉시 사용)
        handSystem?.TryUseCard(_card);
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

    /////////////////




    public void GetDeckCards()
    {
        List<CardDataInstance> temp;

        // 추후 구현
    }

    public void GetWormholeCards()
    {
        List<CardDataInstance> temp;

        // 추후 구현
    }

    public void GetExtinctionCards()
    {
        List<CardDataInstance> temp;

        // 추후 구현
    }

    public void CardDrawed(List<CardDataInstance> cardDataPile)
    {
        ////////////////////////////////////////// 임시
        if (handSystem == null) return;

        for (int i = 0; i < cardDataPile.Count; i++)
        {
            handSystem.ProcessDraw(new Vector2(0, -450f), cardDataPile[i]);
        }
        /////////////////////////////////////////////////


        SetText();
    }





    /////////////////////////////////////////////////



    private void SetText()
    {
        deckCntText.text = "Deck : " + viewCtx.cardSystemProvider.deckCnt.ToString();
        graveCntText.text = "Warmhole : " + viewCtx.cardSystemProvider.graveCnt.ToString();
        handCntText.text = "Hand : " + viewCtx.cardSystemProvider.handCnt.ToString();
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

    public void CardUsed(CardDataInstance usedCard)
    {
        if (viewCtx.cardSystemProvider.CardUsed(usedCard) == false)
            return;



        // 추후 구현


        graveCntText.text = "Warmhole : " + viewCtx.cardSystemProvider.graveCnt.ToString();
    }

    public void CardUsingFinished()
    {
        turnFinishedButton.gameObject.SetActive(false);
        TurnFinishedEvent?.Invoke();

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
}
