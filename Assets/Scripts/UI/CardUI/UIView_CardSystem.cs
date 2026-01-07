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




    [Header("Systems")]
    [SerializeField] private PoolingSystem poolingSystem;
    [SerializeField] private HandSystem handSystem;
    [SerializeField] private DeckSystem deckSystem;
    // [SerializeField] private WormholeSystem WormholeSystem;

    // 덱
    [Header("Deck Settings")]
    [SerializeField] private List<RectTransform> drawPathPoints = new();    // 경로
    [SerializeField] private RectTransform drawEndPoint = null;             // 끝 지점 
    public List<RectTransform> DrawPathPoints { get { return drawPathPoints; } }    // 변수 값 얻기
    public RectTransform DrawEndPoint { get { return drawEndPoint; } }


    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        poolingSystem?.Init(this, handSystem);
        handSystem?.Init(this, poolingSystem);
        deckSystem?.Init(this, poolingSystem);
    }

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
        handSystem.EnqueueDraw(cardDataPile);
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
