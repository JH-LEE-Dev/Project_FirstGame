using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HandSystem : MonoBehaviour
{
    private UIView_CardSystem cardSystem;

    [Header("References")]
    [SerializeField] private RectTransform handRoot;
    [SerializeField] private RectTransform cardSelectHandRoot;
    [SerializeField] private SelectEndButton selectEndButton;
    [SerializeField] private TextMeshProUGUI selectText;


    [Header("Arc Settings")]
    [SerializeField] private float radius;
    [SerializeField] private float minArcAngle;
    [SerializeField] private float maxArcAngle;
    [SerializeField] private float hoverGapWeight;

    private float baseRadius = 2000f;
    private float baseMinArcAngle = 0f;
    private float baseMaxArcAngle = 20f;
    private float baseHoverGapWeight = 0.3f;

    private float selectModeRadius = 4000f;
    private float selectModeMinArcAngle = 0f;
    private float selectModeMaxArcAngle = 15f;
    private float selectModeHoverGapWeight = 0.15f;

    [Header("Select Layout")]
    [SerializeField] private Vector2 selectCenter = Vector2.zero;
    private float selectSpacing = 250f;         
    private float selectMaxWidth = 1200f;
    // 고르는 모드
    private bool bCardSelectMode = false;
    public bool GetChooseMode() { return bCardSelectMode; }
    private int selectMaxCount = 0;
    private bool selectForcing = false;
    private readonly HashSet<int> selectableIdSet  // 선택모드에서 "선택 가능" 판정용
        = new HashSet<int>(); 
    private readonly List<MainCardInstance> hiddenInSelectMode // 선택모드에서 숨겼던 카드들 복구용
        = new List<MainCardInstance>();

    [Header("Preview")]
    [SerializeField] private RectTransform previewRoot;
    private MainCardInstance previewCard; // 미리보기 카드

    [Header("Hand Order")]
    [SerializeField] private List<MainCardInstance> cards = new();

    [Header("Bullet Slots (Temp)")]
    [SerializeField] private List<RectTransform> BulletRoots = new();
    private readonly List<MainCardInstance> equippedBullets = new();

    // 호버된 카드 인덱스
    private MainCardInstance hoveredCard = null;

    public void Init(UIView_CardSystem _cardSystem)
    {
        cardSystem = _cardSystem;

        radius = baseRadius;
        minArcAngle = baseMinArcAngle;
        maxArcAngle = baseMaxArcAngle;
        hoverGapWeight = baseHoverGapWeight;

        selectEndButton.Init(this);
        SettingSelectText(false);
    }

    // 좌클릭 해서 들어온 카드
    public void OnCardLeftClick(MainCardInstance _card)
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
    public void OnCardHoverEnter(MainCardInstance _card)
    {
        if (_card == null) return;
        if (_card.cardState != CardState.InHand) return; // 손패일 때만 벌어짐

        hoveredCard = _card;
        computeArc();
    }

    // 호버 OFF (카드 벌어졌던거 다시 돌아옴)
    public void OnCardHoverExit(MainCardInstance _card)
    {
        if (hoveredCard == _card) hoveredCard = null;

        computeArc();
    }

    // 카드 드로우 (풀링 빌리기)
    public void ProcessDraw(Vector3 _cardSpawnPos, ICardDataInstanceProvider _cardData)
    {
        var card = cardSystem.RentHandCard();
        if (card == null) return;

        // 카드 조립
        card.ApplyData(_cardData);

        // 손패 등록(순서 유지: add가 곧 오른쪽)
        card.gameObject.SetActive(true);
        card.SetUIState(CardState.InHand); 
        card.VisualFloat.PlayDrawColor();

        // 생성 시작 위치
        var rt = card.GetComponent<RectTransform>();
        rt.position = _cardSpawnPos;

        cards.Add(card);

        computeArc();
    }

    private void StartPreview(MainCardInstance card)
    {
        // 기존 프리뷰 있으면 종료(상태 원복)
        if (previewCard != null && previewCard != card)
        {
            previewCard.Motion.EndPreview();
            previewCard.SetUIState(CardState.InHand);
        }


        // 날아가는 동안은 Other임
        previewCard = card;
        previewCard.SetUIState(CardState.Other);
        hoveredCard = null;
        computeArc();


        previewCard.Motion.StartPreview(previewRoot.anchoredPosition, () =>
        {
            // 도중에 다른 카드로 previewCard가 바뀌었으면 무시함
            if (previewCard != card) return;

            card.SetUIState(CardState.Preview);
            computeArc();
        });

    }

    public void CancelPreview(CardState newState = CardState.InHand)
    {
        if (previewCard == null) return;

        previewCard.SetUIState(newState);
        if (newState == CardState.InHand) previewCard.Motion.EndPreview();
        previewCard = null;

        computeArc();
    }

    public void UseCard(MainCardInstance _card, int socketIndex = 0, Transform transform = null)
    {

        if (_card == null) return;
        if (!cards.Contains(_card)) return;


        int idx = cards.IndexOf(_card);
        if (idx < 0) return;

        // 프리뷰 카드 사용이라면 프리뷰 상태 정리
        if (previewCard == _card) previewCard = null;
        // 다른 카드 프리뷰 중이면 취소
        else if (previewCard != null) CancelPreview();

        hoveredCard = null;

        // 재정렬 방어로직
        //if (_card.cardState == CardState.InHand || _card.cardState == CardState.Selecting || _card.cardState == CardState.Preview)
        //    _card.SetUIState(CardState.Other);

        computeArc();
        ComputeSelectedPositions();

        CardType type = _card.CardData.GetCardDataProvider().cardType;

        // 장착 연출하러 고고씽
        if (type == CardType.Bullet) EquipBullet(_card, socketIndex, transform);

    }
    private void EquipBullet(MainCardInstance card, int socketIndex, Transform transform)
    {
        if (card == null) return;
        if (socketIndex < 0) return;

        if (previewCard == card)
        {
            previewCard.Motion.EndPreview();
            previewCard = null;
        }

        bool bIsHand = card.cardState == CardState.InHand ? true : false;

        // 패 레이아웃에서 빠지게 상태 변경
        card.SetUIState(CardState.Other);
        hoveredCard = null;
        computeArc();

        card.Motion.FlyToBulletSocket(bIsHand, transform, () =>
        {
            card.SetUIState(CardState.Equipped);
            card.Motion.SetSocketIndex(socketIndex);
            card.Motion.AllKillTweens(false);
            card.SetVisible(false);

            cardSystem.EquipBulletCard(socketIndex, card.CardData);
        });
    }

    public void UnequipBulletToHand(int socketIndex)
    {
        if (socketIndex < 0) return;


        // 한 소켓에는 여러장이 들어갈 수 있기 때문에, 해당 소켓안에 있던 모든 카드가 복귀한다.
        foreach (var card in cards)
        {
            if (card == null) continue;
            if (card.Motion.socketIndex != socketIndex || CardState.Equipped != card.cardState) continue;

            // Hand 카드 다시 보이게 + 상태 복귀
            card.SetVisible(true);
            card.SetUIState(CardState.InHand);
            // 크기만 다시 패 크기로 돌려놓으면, 알아서 패 레이아웃으로 갈 것임. 
            card.Motion.FlyToHand();
            card.Motion.SetSocketIndex(-1);
        }

        computeArc();
    }
    public void ToggleSelect(MainCardInstance card)
    {
        if (!bCardSelectMode) return;
        if (card == null) return;
        if (card.cardState != CardState.InHand && card.cardState != CardState.Selecting) return;

        // 선택 불가능 카드면 거부 (방어 코드임)
        if (card.cardState == CardState.InHand && !IsSelectableInSelectMode(card))
        {
            card.Motion.PlayReject();
            return;
        }

        // 이미 선택 상태면 해제
        if (card.cardState == CardState.Selecting)
        {
            card.SetUIState(CardState.InHand);
            computeArc();
            ComputeSelectedPositions();
            RefreshSelectEndButton();
            return;
        }

        // InHand에서 Selecting 시도하여 최대 개수를 체크한다.
        int selectedCount = 0;
        foreach (var c in cards)
            if (c != null && c.cardState == CardState.Selecting)
                selectedCount++;

        if (selectedCount >= selectMaxCount)
        {
            // 거부 모션
            card.Motion.PlayReject();
            return;
        }

        // 선택 처리
        card.SetUIState(CardState.Selecting);
        computeArc();
        ComputeSelectedPositions();
        RefreshSelectEndButton();
    }

    private void ComputeSelectedPositions()
    {
        // Selecting 카드들을 cards 리스트 순서로 모음
        List<MainCardInstance> selected = new();

        for (int i = 0; i < cards.Count; i++)
        {
            var c = cards[i];
            if (c != null && c.cardState == CardState.Selecting)
                selected.Add(c);
        }

        int n = selected.Count;
        if (n <= 0) return;

        // 추후 수정.
        Vector2 basePos = selectCenter;

        if (n == 1)
        {
            selected[0].Motion.SetTarget(basePos, 0f);
            selected[0].transform.SetAsLastSibling();
            return;
        }

        float spacing = selectSpacing;
        float needWidth = spacing * (n - 1);
        if (needWidth > selectMaxWidth)
            spacing = selectMaxWidth / (n - 1);

        float startX = -spacing * (n - 1) * 0.5f;

        for (int i = 0; i < n; i++)
        {
            Vector2 pos = basePos + new Vector2(startX + spacing * i, 0f);
            selected[i].Motion.SetTarget(pos, 0f);
            selected[i].transform.SetAsLastSibling();
        }
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

        RectTransform root = bCardSelectMode ? cardSelectHandRoot : handRoot;
        Vector2 basePos = root.anchoredPosition;

        // 프리뷰 중일땐, effectiveHover가 null임. 즉 hoveredCard를 없던일로 한다.
        bool hasPreview = (previewCard != null);
        MainCardInstance effectiveHover = hasPreview ? null : hoveredCard;

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
    private bool IsLayoutExcluded(MainCardInstance c)
    {
        if (c == null) return true;

        // 선택 모드이면서, 핸드에 있고, 선택 불가 카드일땐 호 계산 스킵.
        if (bCardSelectMode && c.cardState == CardState.InHand && !IsSelectableInSelectMode(c))
            return true;

        // 선택모드이면서, 핸드안에서 이펙트 중일 때.
        if (bCardSelectMode && c.cardState == CardState.EffectInHand)
            return true;

        return c.cardState == CardState.Preview
            || c.cardState == CardState.Equipped
            || c.cardState == CardState.Other
            || c.cardState == CardState.EffectOther
            || c.cardState == CardState.Selecting
            || c.cardState == CardState.Hidden;
    }
    private void SortZ_RightIsTop()
    {
        // 오른쪽 카드가 위: cards 리스트 순서대로 SetAsLastSibling
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null) continue;

            // 손패에 남아있는 카드만 정렬(Equipped/Preview는 다른 부모일 수 있음)
            if (cards[i].cardState == CardState.InHand || cards[i].cardState == CardState.EffectInHand)
                cards[i].transform.SetAsLastSibling();
        }
    }



    // For SelectMode

    // 선택 가능 카드 담음.
    private void BuildSelectableSet(CardSelectionModeData selectionData)
    {
        selectableIdSet.Clear();

        if (selectionData.availableCards == null) return;

        for (int i = 0; i < selectionData.availableCards.Count; i++)
        {
            var p = selectionData.availableCards[i];
            if (p == null) continue;

            var data = p.GetCardDataProvider();
            selectableIdSet.Add(data.id);
        }
    }

    // 선택이 가능하다면 true, 선택이 불가하다면 false를 뱉음.
    private bool IsSelectableInSelectMode(MainCardInstance card)
    {
        if (card == null) return false;

        var data = card.CardData.GetCardDataProvider();
        return selectableIdSet.Contains(data.id);
    }


    public void StartCardSelectMode(CardSelectionModeData selectionData, int selectCount, bool bSelectforcing)
    {
        if (bCardSelectMode) return;

        if (previewCard != null) CancelPreview();
        hoveredCard = null;

        int n = Mathf.Max(0, selectCount);

        // 선택가능한 Set 구성함.
        BuildSelectableSet(selectionData);

        bCardSelectMode = true;

        radius = selectModeRadius;
        minArcAngle = selectModeMinArcAngle;
        maxArcAngle = selectModeMaxArcAngle;
        hoverGapWeight = selectModeHoverGapWeight;

        // 요구사항 저장 및 반영
        selectMaxCount = n;
        selectForcing = bSelectforcing;


        // 버튼 및 텍스트 활성
        selectEndButton.SetActiveVisible(true);
        RefreshSelectEndButton();
        SettingSelectText(bCardSelectMode);

        // 선택모드용 숨김 리스트 초기화
        hiddenInSelectMode.Clear();

        // InHand 중 선택 불가능 숨김 처리함
        foreach (var card in cards)
        {
            if (card == null) continue;
            if (card.cardState != CardState.InHand) continue;

            // 선택 불가능한 카드라면
            if (!IsSelectableInSelectMode(card))
            {
                card.SetVisible(false);
                hiddenInSelectMode.Add(card);
                continue;
            }

            // 선택 가능한 카드만 셀렉트 모드 연출 진입
            card.Motion.StartSelectMode();
        }

        computeArc();
        ComputeSelectedPositions();
    }

    public void EndCardSelectMode()
    {
        if (!bCardSelectMode) return;

        if (selectForcing)
        {
            int selectedCount = 0;
            foreach (var c in cards)
                if (c != null && c.cardState == CardState.Selecting)
                    selectedCount++;

            if (selectedCount != selectMaxCount)
                return;
        }

        bCardSelectMode = false;

        // 호 복구
        radius = baseRadius;
        minArcAngle = baseMinArcAngle;
        maxArcAngle = baseMaxArcAngle;
        hoverGapWeight = baseHoverGapWeight;

        if (previewCard != null) CancelPreview();
        hoveredCard = null;

        // 카드 선택 결과 전달함.
        List<ICardDataInstanceProvider> selected = GetSelectedCards();
        cardSystem.EndCardSelectMode(selected);

        // 버튼 및 텍스트 비활성
        selectEndButton.SetActiveVisible(false);
        selectEndButton.SetCanClick(false);
        SettingSelectText(bCardSelectMode);


        // 숨겼던 카드들 복구
        for (int i = 0; i < hiddenInSelectMode.Count; i++)
        {
            var c = hiddenInSelectMode[i];
            if (c == null) continue;
            // 여전히 손패이면 (사실 당연함)
            if (c.cardState == CardState.InHand)
                c.SetVisible(true);
        }

        hiddenInSelectMode.Clear();
        selectableIdSet.Clear();

        foreach (var card in cards)
        {
            if (card == null) continue;
            if (card.cardState == CardState.InHand)
                card.Motion.EndSelectMode();
        }

        computeArc();
        ComputeSelectedPositions();
    }

    private void RefreshSelectEndButton()
    {
        int selectedCount = 0;
        foreach (var c in cards)
            if (c != null && c.cardState == CardState.Selecting)
                selectedCount++;

        bool canClick = selectForcing ? (selectedCount == selectMaxCount) : (selectedCount <= selectMaxCount);
        selectEndButton.SetCanClick(canClick);
    }

    private void SettingSelectText(bool bActive)
    {
        if (!selectText) return;

        selectText.gameObject.SetActive(bActive);
        if (!bActive) return;

        int n = selectMaxCount;

        if (selectForcing)
            selectText.SetText($"카드 {n}장을 선택하세요!");
        else
            selectText.SetText($"카드를 최대 {n}장까지 선택할 수 있어요!");

        var wave = selectText.GetComponent<UIText_SelectTextWave>();
        wave?.Rebuild();
    }

    private List<ICardDataInstanceProvider> GetSelectedCards()
    {
        List<ICardDataInstanceProvider> selected = new();
        foreach (var c in cards)
        {
            if (c != null && c.cardState == CardState.Selecting)
            {
                selected.Add(c.CardData);
            }
        }
        return selected;
    }

    /////////////////////// For Upgrade

    public void UpgradeCard(List<ICardDataInstanceProvider> datas)
    {
        if (datas == null || datas.Count == 0) return;

        HashSet<ICardDataInstanceProvider> dataSet = new HashSet<ICardDataInstanceProvider>(datas);
        foreach (var card in cards)
        {
            if (card == null) continue;

            if (dataSet.Contains(card.CardData))
            {
                card.UpdateForEnforce();
            }
        }
    }


    /////////////////////// For Draw

    public int GetCurrentHandCardCount()
    {
        if (cards == null) return -1;

        int count = 0;
        foreach (var c in cards)
        {
            if (c.cardState == CardState.InHand)
                count++;
        }
        return count;
    }

    // 드로우될 카드 위치
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


    /////////////////////// For Pooling

    // delay = 첫 전체 연출 딜레이, interval = 연속 연출 사이의 간격
    public void ReturnStateAllCard(CardState state, CardReturnType type = CardReturnType.Temp, float delay = 0f, float interval = 0.09f)
    {
        if (previewCard != null) CancelPreview();
        hoveredCard = null;

        List<MainCardInstance> targets = new();

        for (int i = 0; i < cards.Count; i++)
        {
            var c = cards[i];
            if (c != null && c.cardState == state)
                targets.Add(c);
        }

        foreach (var c in targets)
        {
            bool bUseDalay = (type == CardReturnType.FlyToGrave || type == CardReturnType.Extinction || type == CardReturnType.MagicUse);
            float useDelay = bUseDalay ? delay : 0f;

            ReturnCard_Internal(c, type, useDelay);

            if (bUseDalay)
                delay += interval;
        }
    }

    // List에 있는것들 연출.
    public void ReturnCard(List<ICardDataInstanceProvider> cardDataList, CardReturnType type = CardReturnType.Temp, float delay = 0f)
    {
        if (cardDataList == null || cardDataList.Count == 0) return;

        var targetSet = new HashSet<ICardDataInstanceProvider>(cardDataList);

        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var card = cards[i];
            if (card == null) { cards.RemoveAt(i); continue; }

            if (!targetSet.Contains(card.CardData)) continue;

            ReturnCard_Internal(card, type, delay, true);
        }

        computeArc();
    }

    // 연출 후, ReturnToPool
    private void ReturnCard_Internal(MainCardInstance card, CardReturnType type = CardReturnType.Temp, float delay = 0f, bool computeArcOptimization = false)
    {
        if (card == null) return;

        if (previewCard == card) previewCard = null;
        if (hoveredCard == card) hoveredCard = null;

        // 핸드 유지일 경우
        if (type == CardReturnType.StayHand)
        {
            card.SetUIState(CardState.InHand);
            if (computeArcOptimization == false)
            {
                computeArc();
                ComputeSelectedPositions();
            }
            return;
        }


        //card.SetUIState(CardState.Other);
        card.Motion.SetSocketIndex(-1);


        if (!computeArcOptimization)
        {
            computeArc();
            ComputeSelectedPositions();
        }

        bool wasInHand = card.cardState == CardState.InHand;
        switch (type)
        {
            case CardReturnType.Temp:
                ReturnToPool(card);
                break;

            case CardReturnType.FlyToGrave:
                card.SetUIState(CardState.Other);
                PlayFlyToGraveAndReturn(card, delay);
                break;

            case CardReturnType.Extinction:
                if (wasInHand) card.SetUIState(CardState.EffectInHand);
                else card.SetUIState(CardState.EffectOther);
                    PlayExtinctionAndReturn(card, delay);
                break;

            case CardReturnType.MagicUse:
                if (wasInHand) card.SetUIState(CardState.EffectInHand);
                else card.SetUIState(CardState.EffectOther);
                PlayMagicUseAndReturn(card, delay);
                break;
        }
    }

    private void PlayFlyToGraveAndReturn(MainCardInstance card, float delay)
    {
        Vector3 gravePos = cardSystem.GetGraveAnchoredPos();

        DOVirtual.DelayedCall(delay, () =>
        {
            if (card == null) return;
            if (card.cardState == CardState.Hidden) return;

            card.Motion.FlyToGrave(gravePos, () =>
            {
                ReturnToPool(card);
            });

        }).SetUpdate(true);
    }

    private void PlayExtinctionAndReturn(MainCardInstance card, float delay)
    {
        // 예: 소멸 시간/떨림 시간 튜닝
        float dissolveDur = 1.0f;
        float shakeDur = 0.35f;

        DOVirtual.DelayedCall(delay, () =>
        {
            if (card == null) return;
            if (card.cardState == CardState.Hidden) return;

            card.Motion.PlayExtinctionShake(shakeDur);

            card.PlayConsumeExtinction(
                dissolveDur,
                onComplete: (c) =>
                {
                    if (c == null) return;
                    if (c is MainCardInstance mc)
                        ReturnToPool(mc);

                    computeArc();
                    ComputeSelectedPositions();
                });

        }).SetUpdate(true);
    }

    private void PlayMagicUseAndReturn(MainCardInstance card, float delay)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            if (card == null) return;
            if (card.cardState == CardState.Hidden) return;

            float scaleOffset = card.transform.localScale.x * 20f;
            cardSystem.PlayMagicCardEffect(card.transform.position, scaleOffset);

            // 노란 오버레이 강화, 쪼그라짐
            card.VisualFloat?.FadeDrawOverlayAlpha(1f, 0.1f);
            card.Motion.PlayConsumeShrink(0.2f, 0.03f);

            // 마무리 반납
            DOVirtual.DelayedCall(0.6f, () =>
            {
                if (card == null) return;
                if (card.cardState == CardState.Hidden) return;

                // 별똥별/묘지 이펙트 (너 기존 로직)
                Vector2 basePos = card.transform.position;
                Vector2 gravePos = cardSystem.GetGravePos();
                cardSystem.SpawnStarAtoB(false, 0, basePos, gravePos);

                ReturnToPool(card);

                computeArc();
                ComputeSelectedPositions();
            }).SetUpdate(true);

        }).SetUpdate(true);
    }



    // 사용 연출이 전부 끝난 뒤에 호출되는 함수. (단순 풀링 반납)
    private void ReturnToPool(MainCardInstance _card)
    {
        if (_card.cardState == CardState.Hidden) return;
        if (_card == null) return;

        // 카드가 손패 리스트 안에 있으면 제거
        int idx = cards.IndexOf(_card);
        if (idx >= 0) cards.RemoveAt(idx);

        // 장착 리스트에서도 제거
        equippedBullets.Remove(_card);

        // 전부 초기화 한다.
        _card.SetUIState(CardState.Hidden);
        _card.Motion.AllKillTweens();
        _card.VisualFloat.ResetOverlayAlpha();
        _card.gameObject.SetActive(false);

        // 풀링 반납
        cardSystem.ReturnHandCard(_card);
    }

}
