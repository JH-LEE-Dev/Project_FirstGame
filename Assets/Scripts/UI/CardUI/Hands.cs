using System.Collections.Generic;
using UnityEngine;



// 이 클래스는 아직 구현중. 신경 ㄴㄴ
public class Hands : MonoBehaviour
{
    private UIView_CardSystem owner;

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


    public void Init(UIView_CardSystem cardSystem)
    {
        owner = cardSystem;
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
