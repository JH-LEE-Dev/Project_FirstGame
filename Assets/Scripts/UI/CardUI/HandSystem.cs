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

    [Header("Preview")]
    [SerializeField] private RectTransform previewRoot;
    [SerializeField] private float previewScale = 3f;
    [SerializeField] private float previewMoveDuration = 0.12f;
    [SerializeField] private float previewScaleDuration = 0.12f;
    private CardInstance previewCard;   // 현재 미리보기 카드

    [Header("Hand")]
    [SerializeField] private List<CardInstance> cards = new();

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    // 우클릭 해서 들어온 카드.
    public void TryUseCard(CardInstance _card)
    {
        // 이미 다른 카드 프리뷰 중이라면 종료 후 사용.
        if (previewCard != null && previewCard != _card)
            CancelPreview();

        UseCard(_card);
    }
    // 좌클릭 해서 들어온 카드
    public void OnCardLeftClick(CardInstance _card)
    {
        // 만약, 좌클릭한 상태인 카드가 프리뷰 상태일 경우 즉시 사용.
        if (previewCard == _card)
        {
            cardSystem.TryUseCard(_card);
            return;
        }

        // 이 카드의 프리뷰 시작.
        StartPreview(_card);
    }

    private void StartPreview(CardInstance card)
    {
        // 프리뷰 중인 카드가 이미 존재할 경우, 기존 카드의 프리뷰를 종료한다.
        if (previewCard != null && previewCard != card)
            previewCard.EndPreview();

        // 새로운것으로 교체.
        previewCard = card;

        // 프리뷰 시작(센터 이동 + 확대)
        previewCard.StartPreview(previewRoot.anchoredPosition, previewScale, previewMoveDuration, previewScaleDuration);



        computeArc();
    }

    public void CancelPreview()
    {
        if (previewCard == null) return;

        // 카드 비주얼만 원래대로
        previewCard.EndPreview();
        previewCard = null;

        computeArc();
    }

    private void UseCard(CardInstance _card)
    {
        int idx = cards.IndexOf(_card);
        if (idx < 0) return;


        if (previewCard == _card)
        {
            // 프리뷰 상태 종료 (ignoreHandLayout false로 복귀)
            previewCard.EndPreview();
            previewCard = null;
        }
        else if (previewCard != null)
        {
            // 다른 카드 프리뷰 중인데 다른 카드를 사용한다? : 프리뷰 취소
            CancelPreview();
        }

        // 호버링 초기화.
        hoveredIndex = -1;


        cards.RemoveAt(idx);

        // 전부 초기화 한다.
        _card.ExitHand();
        _card.gameObject.SetActive(false); // 임시, 연출 후 비활성으로...
        // 풀링 반납
        cardSystem.ReturnHandCard(_card);

        // 호 재계산
        computeArc();
    }

    public void ProcessDraw(Vector3 _cardSpawnPos, CardDataInstance _cardData)
    {
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
        // 프리뷰 방어코드
        if (previewCard != null && !cards.Contains(previewCard))
            previewCard = null;

        int total = cards.Count;
        if (total <= 0) return;

        Vector2 basePos = handRoot.anchoredPosition;

        // 프리뷰 중이면 해당 카드는 레이아웃에서 제외
        bool hasPreview = (previewCard != null);
        int layoutCount = hasPreview ? (total - 1) : total;

        // 레이아웃에 배치할 카드가 0장인 경우 (프리뷰만 있는 경우)
        if (layoutCount <= 0) return;

        // 프리뷰 중엔 호버 벌리기 기능 끄기
        int effectiveHoveredIndex = hasPreview ? -1 : hoveredIndex;

        // 1장이면 중앙
        if (layoutCount == 1)
        {
            // preview 아닌 카드 1장 찾아서 중앙
            for (int i = 0; i < total; i++)
            {
                var c = cards[i];
                if (hasPreview && c == previewCard) continue;
                c.UpdateTargetPos(basePos, 0f);
                break;
            }
            return;
        }

        float t = Mathf.InverseLerp(0f, 12f, layoutCount);
        float arcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, t);

        float angleStep = arcAngle / Mathf.Max(1, layoutCount - 1);
        float startAngle = -arcAngle * 0.5f;

        int layoutIndex = 0;

        for (int i = 0; i < total; i++)
        {
            var card = cards[i];

            // 프리뷰 카드는 레이아웃에서 제외
            if (hasPreview && card == previewCard)
                continue;

            float offset = 0f;

            // 호버 벌리기 (프리뷰 중엔 꺼짐)
            if (effectiveHoveredIndex >= 0 && hoverGapWeight > 0f)
            {
                if (layoutIndex > effectiveHoveredIndex) offset += hoverGapWeight;
                else if (layoutIndex < effectiveHoveredIndex) offset -= hoverGapWeight;

                if (effectiveHoveredIndex == 0 && layoutIndex > effectiveHoveredIndex)
                    offset -= hoverGapWeight * 0.5f;

                if (effectiveHoveredIndex == layoutCount - 1 && layoutIndex < effectiveHoveredIndex)
                    offset += hoverGapWeight * 0.5f;
            }

            float angle = startAngle + angleStep * (layoutIndex + offset);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = basePos + new Vector2(
                Mathf.Sin(rad) * radius,
                (Mathf.Cos(rad) - 1f) * radius
            );

            float tiltZ = -angle * 0.8f;

            card.UpdateTargetPos(pos, tiltZ);

            layoutIndex++;
        }

        SortZ_RightIsTop();
    }

    private void SortZ_RightIsTop()
    {
        for (int i = 0; i < cards.Count; i++)
            cards[i].transform.SetAsLastSibling();
    }
}
