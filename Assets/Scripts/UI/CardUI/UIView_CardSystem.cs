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



    // 패 기능
    [Header("References")]
    [SerializeField] private RectTransform handRoot;
    [Header("Arc Settings")]
    [SerializeField] private float radius = 2000f;
    [SerializeField] private float minArcAngle = 0f;
    [SerializeField] private float maxArcAngle = 20f;
    [SerializeField] private float hoverGapWeight = 0.3f;
    private int hoveredIndex = -1;
    // 실제 나의 패
    [SerializeField] private List<CardInstance> cards = new();




    // for System
    [Header("Pooling")]
    // 카드 기본 프리팹
    [SerializeField] private GameObject cardUIPrefab;

    // 비활성중인 패
    [SerializeField] private List<CardInstance> inactiveHandPool = new();
    // 활성중인 패
    [SerializeField] private List<CardInstance> activeHandCards = new();
    private int maxHandPool = 20;

    // 소멸, 웜홀, 덱
    [SerializeField] private List<CardInstance> otherCardPool = new();
    private int maxOtherCardPool = 50;


    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);

        cardPooling();

    }

    private void cardPooling()
    {
        // hands
        for (int i = 0; i < maxHandPool; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, this.transform);
            CardInstance card = go.GetComponent<CardInstance>();
            card.gameObject.SetActive(false);

            card.Initialize(this);
            inactiveHandPool.Add(card);

        }

        // other
        for (int i = 0; i < maxOtherCardPool; ++i)
        {
            GameObject go = Instantiate(cardUIPrefab, this.transform);
            CardInstance card = go.GetComponent<CardInstance>();
            card.gameObject.SetActive(false);

            card.Initialize(this);
            otherCardPool.Add(card);
        }
    }

    public void GetDeckCards()
    {
        List<CardDataInstance> temp;

        // 수정 요망
    }

    public void GetWormholeCards()
    {
        List<CardDataInstance> temp;

        // 수정 요망
    }

    public void GetExtinctionCards()
    {
        List<CardDataInstance> temp;

        // 수정 요망
    }

    // 카드 드로우 (패 입성)
    public void CardDrawed(List<CardDataInstance> cardDataPile)
    {
        foreach (var data in cardDataPile)
        {
            if (inactiveHandPool.Count <= 0) return;

            // 앞에서 부터 사용.
            CardInstance card = inactiveHandPool[0];
            inactiveHandPool.RemoveAt(0);

            //card.gameObject.SetActive(true);
            card.ApplyData(data);

            activeHandCards.Add(card);
        }

        SetText();
    }

    // 전체 초기화
    public void ReleaseAllHand()
    {
        foreach (var card in activeHandCards)
        {
            card.Clear();
            //card.gameObject.SetActive(false);
            inactiveHandPool.Add(card);
        }

        activeHandCards.Clear();
    }

    // 하나 제거
    public void ReleaseHandAt(int index)
    {
        if (index < 0 || index >= activeHandCards.Count)
            return;

        CardInstance card = activeHandCards[index];
        activeHandCards.RemoveAt(index);

        card.Clear();
        //card.gameObject.SetActive(false);

        inactiveHandPool.Add(card);
    }

    private void SetText()
    {
    }





    protected override void OnShow()
    {
        base.OnShow();

        //computeArc();

        SetText();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void RenderUI()
    {

    }

    // 수정 요망

    public void CardUsed(CardDataInstance usedCard)
    {
        //if (viewCtx.cardSystemProvider.CardUsed(usedCard) == false)
        //    return;

        //usedCard.gameObject.SetActive(false);
        //cards.Remove(usedCard);
        //computeArc();
        //graveCntText.text = "Warmhole : " + viewCtx.cardSystemProvider.graveCnt.ToString();
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
        handRoot.gameObject.SetActive(false);
    }

    public void PlayerTurnStarted(int waveIdx)
    {
        handRoot.gameObject.SetActive(true);
    }





    // 호버 ON (카드 약간 벌어짐)
    public void OnCardHoverEnter(CardInstance card)
    {
        hoveredIndex = cards.IndexOf(card);

        //computeArc();
    }

    // 호버 OFF (카드 벌어졌던거 다시 돌아옴)
    public void OnCardHoverExit(CardInstance card)
    {
        int idx = cards.IndexOf(card);
        if (idx == hoveredIndex) hoveredIndex = -1;

        //computeArc();
    }


}
