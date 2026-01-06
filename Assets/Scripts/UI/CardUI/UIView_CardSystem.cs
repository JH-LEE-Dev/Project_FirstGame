using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIView_CardSystem : UIView
{
    public event Action TurnFinishedEvent;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject cardUIPrefab;
    [Space]
    [SerializeField] private TMP_Text deckCntText;
    [SerializeField] private TMP_Text graveCntText;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button turnFinishedButton;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;
    [SerializeField] private List<CardInstance> cards = new();


    [Header("Arc Settings")]
    [SerializeField] private float radius = 2000f;
    [SerializeField] private float minArcAngle = 0f;
    [SerializeField] private float maxArcAngle = 20f;
    [SerializeField] private float hoverGapWeight = 0.3f;

    private int hoveredIndex = -1;


    protected override void Awake()
    {
        base.Awake();

        SetAnchorToCanvas(uiRoot.transform);

        turnFinishedButton.onClick.AddListener(CardUsingFinished);
        turnFinishedButton.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        base.OnShow();

        deckCntText.text = "Deck : " + viewCtx.cardSystemProvider.deckCnt.ToString();
        graveCntText.text = "Warmhole : " + viewCtx.cardSystemProvider.graveCnt.ToString();

        computeArc();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void RenderUI()
    {

    }

    // 호버 ON (카드 약간 벌어짐)
    public void OnCardHoverEnter(CardInstance card)
    {
        hoveredIndex = cards.IndexOf(card);

        computeArc();
    }

    // 호버 OFF (카드 벌어졌던거 다시 돌아옴)
    public void OnCardHoverExit(CardInstance card)
    {
        int idx = cards.IndexOf(card);
        if (idx == hoveredIndex) hoveredIndex = -1;

        computeArc();
    }

    /*수정 요망*/
    public void CardDrawed(CardDataInstance cardInstance)
    {
        //cardInstance.gameObject.SetActive(true);
        //cardInstance.GetComponent<RectTransform>().SetParent(this.transform, false);
        //cardInstance.CardUsedEvent -= CardUsed;
        //cardInstance.CardUsedEvent += CardUsed;


        // cards 
        //cards.Add(cardInstance);

        // ★상우★ 일단 여기에 카드가 Active 시작되는 위치.
        //RectTransform rt = cardInstance.GetComponent<RectTransform>();
       // rt.anchoredPosition = new Vector2(300f, -150f);

        // ★정현★ 패 매니저를 카드에게 참조시킨다. 
        //cardInstance.SetMaker(this);
        //computeArc();

        deckCntText.text = "Deck : " + viewCtx.cardSystemProvider.deckCnt.ToString();
    }

    // 호를 구성해서, 카드들에게 좌표랑 각도를 던져준다.
    public void computeArc()
    {
        int n = cards.Count;
        if (n <= 0) return;

        Vector2 basePos = handRoot.anchoredPosition;

        // 1장이면 중앙 고정
        if (n == 1)
        {
            cards[0].UpdateTargetPos(basePos, 0f);
            return;
        }

        float t = Mathf.InverseLerp(0f, 12f, n);
        float arcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, t);

        float angleStep = arcAngle / Mathf.Max(1, n - 1);
        float startAngle = -arcAngle * 0.5f;

        for (int i = 0; i < n; i++)
        {
            float offset = 0f;

            if (hoveredIndex >= 0 && hoverGapWeight > 0f)
            {
                if (i > hoveredIndex)
                    offset += hoverGapWeight;
                else if (i < hoveredIndex)
                    offset -= hoverGapWeight;

                if (hoveredIndex == 0 && i > hoveredIndex)
                    offset -= hoverGapWeight * 0.5f;

                if (hoveredIndex == n - 1 && i < hoveredIndex)
                    offset += hoverGapWeight * 0.5f;
            }

            float angle = startAngle + angleStep * (i + offset);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = basePos + new Vector2(
                Mathf.Sin(rad) * radius,
                (Mathf.Cos(rad) - 1f) * radius
            );

            float tiltZ = -angle * 0.8f;

            cards[i].UpdateTargetPos(pos, tiltZ);
        }
    }

    /*수정 요망*/
    public void CardUsed(CardDataInstance usedCard)
    {
        if (viewCtx.cardSystemProvider.CardUsed(usedCard) == false)
            return;

        //usedCard.gameObject.SetActive(false);
        //cards.Remove(usedCard);
        computeArc();
        graveCntText.text = "Warmhole : " + viewCtx.cardSystemProvider.graveCnt.ToString();
    }

    public void CardUsingFinished()
    {
        turnFinishedButton.gameObject.SetActive(false);
        TurnFinishedEvent?.Invoke();

        
        deckCntText.text = "Deck : " + viewCtx.cardSystemProvider.deckCnt.ToString();
        graveCntText.text = "Warmhole : " + viewCtx.cardSystemProvider.graveCnt.ToString();

        ClearAllCards();
    }

    public void ClearAllCards()
    {
        for(int i = 0; i < cards.Count;++i)
        {
            RectTransform card = cards[i].GetComponent<RectTransform>();

            card.localPosition = new Vector3(-1000,-1000,card.localPosition.z);
        }

        cards.Clear();

        computeArc();
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
}
