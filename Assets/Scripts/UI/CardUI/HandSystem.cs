using DG.Tweening;
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

    [Header("Preview")]
    [SerializeField] private RectTransform previewRoot;
    private CardInstance previewCard; // 미리보기 카드

    [Header("Hand Order")]
    [SerializeField] private List<CardInstance> cards = new();

    [Header("Bullet Slots (Temp)")]
    [SerializeField] private List<RectTransform> BulletRoots = new();
    private readonly List<CardInstance> equippedBullets = new();

    // 호버된 카드 인덱스
    private CardInstance hoveredCard = null;

    [Header("ToGrave")]
    [SerializeField] private float discardInterval = 0.09f;



    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;
    }

    // 좌클릭 해서 들어온 카드
    public void OnCardLeftClick(CardInstance _card)
    {
        if (_card == null) return;

        // 프리뷰 상태에서 다시 클릭 -> 사용
        if (previewCard == _card)
        {
            cardSystem.TryUseCard(_card);
            return;
        }

        // 손패 카드만 프리뷰 가능
        if (_card.cardState != CardState.InHand)
            return;

        StartPreview(_card);
    }

    // 호버 ON (카드 약간 벌어짐)
    public void OnCardHoverEnter(CardInstance _card)
    {
        if (_card == null) return;
        if (_card.cardState != CardState.InHand) return; // 손패일 때만 벌어짐

        hoveredCard = _card;
        computeArc();
    }

    // 호버 OFF (카드 벌어졌던거 다시 돌아옴)
    public void OnCardHoverExit(CardInstance _card)
    {
        if (hoveredCard == _card) hoveredCard = null;

        computeArc();
    }

    // 카드 드로우 (풀링 빌리기)
    public void ProcessDraw(Vector3 _cardSpawnPos, CardDataInstance _cardData)
    {
        var card = cardSystem.RentHandCard();
        if (card == null) return;

        // 카드 조립
        card.ApplyData(_cardData);

        // 손패 등록(순서 유지: add가 곧 오른쪽)
        card.gameObject.SetActive(true);
        card.SetUIState(CardState.InHand); 
        
        card.Motion.EnterHand();
        card.VisualFloat.PlayDrawColor();

        // 생성 시작 위치
        var rt = card.GetComponent<RectTransform>();
        rt.position = _cardSpawnPos;

        cards.Add(card);

        computeArc();
    }

    private void StartPreview(CardInstance card)
    {
        // 기존 프리뷰 있으면 종료(상태 원복)
        if (previewCard != null && previewCard != card)
        {
            previewCard.Motion.EndPreview();
            previewCard.SetUIState(CardState.InHand);
        }


        previewCard = card;
        previewCard.SetUIState(CardState.Preview);

        previewCard.Motion.StartPreview(previewRoot.anchoredPosition);

        hoveredCard = null;

        computeArc();
    }

    public void CancelPreview()
    {
        if (previewCard == null) return;

        previewCard.SetUIState(CardState.InHand);
        previewCard.Motion.EndPreview();
        previewCard = null;

        computeArc();
    }

    public void UseCard(CardInstance _card)
    {
        if (_card == null) return;

        int idx = cards.IndexOf(_card);
        if (idx < 0) return;


        // 프리뷰 카드 사용이라면 프리뷰 상태 정리
        if (previewCard == _card)
        {
            previewCard.Motion.EndPreview();
            previewCard.SetUIState(CardState.Other); // Other는 연출 중인 놈을 의미함. 자유분방
            previewCard = null;
        }
        else if (previewCard != null)
        {
            // 다른 카드 프리뷰 중이면 취소
            CancelPreview();
        }

        hoveredCard = null;

        CardType type = _card.CardData.GetCardData().cardType;

        switch (type)
        {
            case CardType.Bullet:
                ConsumeMagic(_card); // 임시..
                //EquipBullet(_card);
                break;

            case CardType.Magic:
                ConsumeMagic(_card);
                break;
        }

    }

    private void EquipBullet(CardInstance card)
    {
        // 손패또는 프리뷰 상태에서만 장착 허용
        if (false == (card.cardState == CardState.InHand || card.cardState == CardState.Preview))
            return;

        // 빈 슬롯 찾기
        int slot = FindEmptyBulletSlot();
        if (slot < 0)
        {
            card.Motion.PlayReject();
            return;
        }

        // 상태 전이
        card.SetUIState(CardState.Equipped);
        equippedBullets.Add(card);

        // 필드로 이동
        card.Motion.ExitHand();
        card.transform.SetParent(BulletRoots[slot], false);

        // 슬롯 중앙에 고정(임시)
        var rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;

        computeArc();
    }

    private int FindEmptyBulletSlot()
    {
        if (BulletRoots == null || BulletRoots.Count == 0) return -1;

        // 슬롯에 이미 카드가 붙어있으면 occupied로 판단
        for (int i = 0; i < BulletRoots.Count; i++)
        {
            bool occupied = false;
            for (int j = 0; j < equippedBullets.Count; j++)
            {
                if (equippedBullets[j] != null && equippedBullets[j].transform.parent == BulletRoots[i])
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied) return i;
        }
        return -1;
    }

    private void ConsumeMagic(CardInstance card)
    {
        // 연출 자리: 여기서 셰이더/불타기 끝나면 Return

        // 지금은 즉시 반환
        ReturnToPool(card);
    }


    // 패 풀링으로부터 생성
    // 패 풀링한테 반납
    private void ReturnToPool(CardInstance _card)
    {
        if (_card == null) return;

        // 카드가 손패 리스트 안에 있으면 제거 (소모형만 제거하는 흐름)
        int idx = cards.IndexOf(_card);
        if (idx >= 0) cards.RemoveAt(idx);

        // 장착 리스트에서도 제거
        equippedBullets.Remove(_card);




        // 전부 초기화 한다.
        _card.SetUIState(CardState.Hidden);
        _card.Motion.ExitHand();
        _card.gameObject.SetActive(false);

        // 풀링 반납
        cardSystem.ReturnHandCard(_card);

        // 호 재계산
        computeArc();
    }

    // 장착한 불릿을 다시 손패로 돌리고 싶을 때
    public void UnequipBulletToHand(CardInstance card)
    {
        if (card == null) return;
        if (card.cardState != CardState.Equipped) return;

        equippedBullets.Remove(card);

        card.transform.SetParent(handRoot, false);
        card.SetUIState(CardState.InHand);
        card.Motion.EnterHand();

        computeArc();
    }

    private bool IsLayoutExcluded(CardInstance c)
    {
        if (c == null) return true;

        return c.cardState == CardState.Preview
            || c.cardState == CardState.Equipped
            || c.cardState == CardState.Hidden
            || c.cardState == CardState.Other;
    }


    // 호를 구성해서, 카드들에게 좌표랑 각도를 던져준다.
    private void computeArc()
    {
        if (previewCard != null && !cards.Contains(previewCard))
            previewCard = null;


        // 패만 카운트 한다.
        int layoutCount = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (!IsLayoutExcluded(cards[i]))
                layoutCount++;
        }

        // 패가 0장 이하라면, 연산을 할 필요가 없다.
        if (layoutCount <= 0) return;


        Vector2 basePos = handRoot.anchoredPosition;

        // 프리뷰 중일땐, effectiveHover가 null임. 즉 hoveredCard를 없던일로 한다.
        bool hasPreview = (previewCard != null);
        CardInstance effectiveHover = hasPreview ? null : hoveredCard;

        // 만약, 패가 하나라면 바로 가운데에 박아버리고 연산을 하지 않는다.
        if (layoutCount == 1)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var c = cards[i];
                if (IsLayoutExcluded(c)) continue;
                c.Motion.SetTarget(basePos, 0f);
                break;
            }
            SortZ_RightIsTop();
            return;
        }

        float t = Mathf.InverseLerp(0f, 12f, layoutCount);
        float arcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, t);

        float angleStep = arcAngle / Mathf.Max(1, layoutCount - 1);
        float startAngle = -arcAngle * 0.5f;

        int layoutIndex = 0;
        int hoveredLayoutIndex = -1;

        // 위에서 effectiveHover(hoveredCard) 가 있었다면..
        // tmp가상의 인덱스를 이용해서 hoveredLayoutIndex를 재구성.
        if (effectiveHover != null)
        {
            int tmp = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                var c = cards[i];
                if (IsLayoutExcluded(c)) continue;
                if (c == effectiveHover) { hoveredLayoutIndex = tmp; break; }
                tmp++;
            }
        }

        // IsLayoutExcluded에 걸러지지 않은 카드들만.
        // hoveredLayoutIndex 재구성을 통해 계산하여 카드에게 위치정보를 전달한다.
        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            if (IsLayoutExcluded(card)) continue;

            float offset = 0f;

            if (hoveredLayoutIndex >= 0 && hoverGapWeight > 0f)
            {
                if (layoutIndex > hoveredLayoutIndex) offset += hoverGapWeight;
                else if (layoutIndex < hoveredLayoutIndex) offset -= hoverGapWeight;

                if (hoveredLayoutIndex == 0 && layoutIndex > hoveredLayoutIndex)
                    offset -= hoverGapWeight * 0.5f;

                if (hoveredLayoutIndex == layoutCount - 1 && layoutIndex < hoveredLayoutIndex)
                    offset += hoverGapWeight * 0.5f;
            }

            float angle = startAngle + angleStep * (layoutIndex + offset);
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = basePos + new Vector2(
                Mathf.Sin(rad) * radius,
                (Mathf.Cos(rad) - 1f) * radius
            );

            float tiltZ = -angle * 0.8f;

            card.Motion.SetTarget(pos, tiltZ);

            layoutIndex++;
        }

        SortZ_RightIsTop();
    }

    private void SortZ_RightIsTop()
    {
        // 오른쪽 카드가 위: cards 리스트 순서대로 SetAsLastSibling
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;

            // 손패에 남아있는 카드만 정렬(Equipped/Preview는 다른 부모일 수 있음)
            if (cards[i].cardState == CardState.InHand)
                cards[i].transform.SetAsLastSibling();
        }
    }

    public int GetCurrentHandCardCount() => cards.Count;

    public Vector2 PredictRightmostPosForCount(int nextCount)
    {
        Vector2 basePos = handRoot.position;
        if (nextCount <= 1) return basePos;

        float t = Mathf.InverseLerp(0f, 12f, nextCount);
        float arcAngle = Mathf.Lerp(minArcAngle, maxArcAngle, t);

        float angleStep = arcAngle / Mathf.Max(1, nextCount - 1);
        float startAngle = -arcAngle * 0.5f;

        int rightIndex = nextCount - 1;

        float angle = startAngle + angleStep * rightIndex;
        float rad = angle * Mathf.Deg2Rad;

        Vector2 localOffset = new Vector2(Mathf.Sin(rad) * radius, (Mathf.Cos(rad) - 1f) * radius);
        return handRoot.TransformPoint(localOffset);
    }

    public int CurrentHandCount()
    {
        if (cards == null) return -1;

        int Count = 0;

        foreach(var card in cards)
        {
            if (card.cardState == CardState.InHand)
                Count++;
        }

        return Count;
    }

    public void AllCardReturnToPool(CardState state)
    {
        if (previewCard != null) CancelPreview();
        hoveredCard = null;

        Vector3 GravePosition = cardSystem.GetGraveAnchoredPos();

        Debug.Log(GravePosition);


        // 현재 손패(InHand)만 스냅샷
        List<CardInstance> toDiscard = new();

        for (int i = 0; i < cards.Count; i++)
        {
            var c = cards[i];
            if (c != null && c.cardState == state)
                toDiscard.Add(c);
        }

        float delay = 0f;

        foreach (var card in toDiscard)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                if (card == null) return;
                if (card.cardState != CardState.InHand) return;

                // 손패 레이아웃에서 즉시 제외
                card.SetUIState(CardState.Other);
                computeArc();

                card.Motion.FlyToGrave(GravePosition, () =>
                {
                    ReturnToPool(card);
                });

            }).SetUpdate(true);

            delay += discardInterval;
        }

    }
}
