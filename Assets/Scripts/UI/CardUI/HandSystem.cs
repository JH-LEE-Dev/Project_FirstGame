using System.Collections.Generic;
using UnityEngine;


public class HandSystem : MonoBehaviour
{
    private UIView_CardSystem cardSystem;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;

    [Header("Arc Settings")]
    [SerializeField] private float radius = 2000f;
    [SerializeField] private float minArcAngle = 0f;
    [SerializeField] private float maxArcAngle = 20f;
    [SerializeField] private float hoverGapWeight = 0.3f;
    // 호버된 카드 인덱스
    private int hoveredIndex = -1;

    [Header("Hand")]
    [SerializeField] private List<CardInstance> cards = new();

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    public void UseCard(CardInstance _card)
    {
        int idx = cards.IndexOf(_card);
        if (idx < 0) return;

        UseCardAt(idx);
    }

    public void UseCardAt(int _index)
    {
        if (_index < 0 || _index >= cards.Count) return;

        var card = cards[_index];
        cards.RemoveAt(_index);

        if (hoveredIndex == _index) hoveredIndex = -1;
        else if (hoveredIndex > _index) hoveredIndex--;

        card.ExitHand();
        card.gameObject.SetActive(false); // 임시, 연출 후 비활성으로...

        cardSystem.ReturnHandCard(card);

        computeArc();
    }

    public void ProcessDraw(Vector3 _cardSpawnPos, CardDataInstance _cardData)
    {
        Debug.Log("호출");

        var card = cardSystem.RentHandCard();
        if (card == null) return;

        // 카드 조립
        card.ApplyData(_cardData);

        // 손 패로 이동.
        card.gameObject.SetActive(true);
        card.EnterHand();

        // 덱 위치에서 시작시키기 (파다다닥 출발점)
        var rt = card.GetComponent<RectTransform>();
        rt.position = _cardSpawnPos;

        cards.Add(card);
        computeArc();
    }

    // 호버 ON (카드 약간 벌어짐)
    public void OnCardHoverEnter(CardInstance _card)
    {
        hoveredIndex = cards.IndexOf(_card);

        computeArc();
    }

    // 호버 OFF (카드 벌어졌던거 다시 돌아옴)
    public void OnCardHoverExit(CardInstance _card)
    {
        int idx = cards.IndexOf(_card);
        if (idx == hoveredIndex) hoveredIndex = -1;

        computeArc();
    }

    // 호를 구성해서, 카드들에게 좌표랑 각도를 던져준다.
    private void computeArc()
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
