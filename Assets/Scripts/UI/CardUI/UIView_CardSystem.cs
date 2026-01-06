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
    [SerializeField] private GameObject uiPrefab;
    [Space]
    [SerializeField] private TMP_Text deckCntText;
    [SerializeField] private TMP_Text graveCntText;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button turnFinishedButton;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;
    [SerializeField] private List<CardInstance> cards = new();


    [Header("Fan Settings")]
    [SerializeField] private float radius = 2000f;
    [SerializeField] private float minArcAngle = 0f;
    [SerializeField] private float maxArcAngle = 20f;

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


    public void CardDrawed(CardInstance cardInstance)
    {
        cardInstance.gameObject.SetActive(true);
        cardInstance.GetComponent<RectTransform>().SetParent(handRoot, false);
        cardInstance.CardUsedEvent -= CardUsed;
        cardInstance.CardUsedEvent += CardUsed;


        // cards 
        cards.Add(cardInstance);

        // ★상우★ 일단 여기에 카드가 Active 시작되는 위치.
        RectTransform rt = cardInstance.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(300f, -150f);

        // ★정현★ 패 매니저를 카드에게 참조시킨다. 
        cardInstance.SetMaker(this);
        computeArc();

        deckCntText.text = "Deck : " + viewCtx.cardSystemProvider.deckCnt.ToString();
    }

    // 호를 구성해서, 카드들에게 좌표랑 각도를 던져준다.
    public void computeArc()
    {

        // 호 계산
        int n = cards.Count;
        if (n <= 0) return;

        // 기준점
        Vector2 basePos = handRoot.anchoredPosition;

        // 1장이면 중앙 고정
        if (n == 1)
        {
            cards[0].UpdateTargetPos(basePos, 0);
            return;
        }

        int addSlots =
        (hoveredIndex < 0) ? 0 :
        (hoveredIndex == 0 || hoveredIndex == n - 1) ? 1 : 2;

        int slotCount = n + addSlots;


        float effectiveSlots = n + addSlots * 0.5f;

        float t = Mathf.InverseLerp(0f, 12f, effectiveSlots);
        float baseArcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, t);

        float arcAngle = baseArcAngle; // 추가 배율 없이도 충분히 자연스러움

        float angleStep = arcAngle / Mathf.Max(1, slotCount - 1);
        float startAngle = -arcAngle * 0.5f;

        int cardIdx = 0;

        for (int slot = 0; slot < slotCount; slot++)
        {
            bool skipSlot = false;

            if (hoveredIndex >= 0)
            {
                if (addSlots == 2)
                {
                    skipSlot =
                        slot == hoveredIndex ||
                        slot == hoveredIndex + 2;
                }
                else if (hoveredIndex == 0)
                {
                    skipSlot = slot == 1;
                }
                else if (hoveredIndex == n - 1)
                {
                    skipSlot = slot == hoveredIndex;
                }
            }

            if (skipSlot)
                continue;

            float angle = startAngle + angleStep * slot;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = basePos + new Vector2(
                Mathf.Sin(rad) * radius,
                (Mathf.Cos(rad) - 1f) * radius
            );

            float tiltZ = -angle * 0.8f;


            // 여기서 던져주는거임.
            cards[cardIdx].UpdateTargetPos(pos, tiltZ);
            cardIdx++;
        }
    }

    public void CardUsed(CardInstance usedCard)
    {
        if (viewCtx.cardSystemProvider.CardUsed(usedCard) == false)
            return;

        usedCard.gameObject.SetActive(false);
        cards.Remove(usedCard);
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
