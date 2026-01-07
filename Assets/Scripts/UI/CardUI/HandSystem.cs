using System.Collections.Generic;
using UnityEngine;


public class HandSystem : MonoBehaviour
{
    private UIView_CardSystem owner;
    private PoolingSystem poolingSystem;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;

    [Header("Arc Settings")]
    [SerializeField] private float radius = 2000f;
    [SerializeField] private float minArcAngle = 0f;
    [SerializeField] private float maxArcAngle = 20f;
    [SerializeField] private float hoverGapWeight = 0.3f;
    private int hoveredIndex = -1;

    [Header("Hand")]
    [SerializeField] private List<CardInstance> cards = new();
    [SerializeField] Queue<CardDataInstance> drawQueue = new();
    [SerializeField] private float drawStagger = 0.06f; // 파다다닥 간격
    private float drawTimer;


    public void Init(UIView_CardSystem cardSystem)
    {
        owner = cardSystem;
    }

    public void EnqueueDraw(List<CardDataInstance> datas)
    {
        foreach (var d in datas) drawQueue.Enqueue(d);
    }

    private void Update()
    {
        // 임시로 매프레임으로 하는중 신경 ㄴㄴ
        ProcessDrawQueue();

        // 임시 신경 ㄴㄴ
        computeArc();

    }

    private void ProcessDrawQueue()
    {
        if (drawQueue.Count == 0) return;

        drawTimer -= Time.unscaledDeltaTime;
        if (drawTimer > 0f) return;

        var data = drawQueue.Dequeue();

        var card = poolingSystem.RentHandCard();
        if (card == null) return;

        // 여기서부터가 연출 책임
        card.ApplyData(data);

        // 아직 손패 연출로 들어가기 전 상태 세팅
        card.gameObject.SetActive(true);
        card.inHand = true;

        // 덱 위치에서 시작시키기 (파다다닥 출발점)
        var rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -450f);

        cards.Add(card);
        //computeArc();

        drawTimer = drawStagger;
    }


    public void AddCards(List<CardInstance> newCards)
    {
        cards.AddRange(newCards);
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
}
