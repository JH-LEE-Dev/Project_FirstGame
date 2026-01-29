using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UICommandFactory_CardSystem : UICommandFactory
{
    public delegate void CardLogicSystemCommandCreator(CardSystemContextType cardSystemContextType, ReadOnlySpan<CardDataInstance> cards);
    private CardLogicSystemCommandCreator[] cardLogicSystemCreatorMap;
    public delegate void CardDataControlSystemCommandCreator(CardSystemContextType cardSystemContextType, ReadOnlySpan<CardDataInstance> cards);
    private CardDataControlSystemCommandCreator[] cardDataControlSystemCreatorMap;

    const int maxBatchPoolSize = 10;
    const int jobListSize = 10;
    const int cardListSize = 30;
    const int batchPoolSize = 5;

    // Job Batch Pool (Job들을 전달하기 위한 용도)
    private ObjectPool<List<CardUIActionData>> jobBatchPool =
        new ObjectPool<List<CardUIActionData>>(
            createFunc: () => new List<CardUIActionData>(jobListSize),
            actionOnGet: (list) => list.Clear(),
            actionOnRelease: (list) => list.Clear(),
            collectionCheck: true,
            defaultCapacity: batchPoolSize,
            maxSize: maxBatchPoolSize
        );

    // CardDataInstance 리스트 풀 (실제 카드 알맹이 담는 용도)
    private ObjectPool<List<CardDataInstance>> cardListPool =
        new ObjectPool<List<CardDataInstance>>(
            createFunc: () => new List<CardDataInstance>(cardListSize),
            actionOnGet: (list) => list.Clear(),
            actionOnRelease: (list) => list.Clear(),
            collectionCheck: true,
            defaultCapacity: batchPoolSize,
            maxSize: maxBatchPoolSize
        );

    private Queue<int> availableSlots = new Queue<int>(maxBatchPoolSize); // 사용 가능한 슬롯 인덱스들
    private List<List<CardUIActionData>> jobSlots = new List<List<CardUIActionData>>(maxBatchPoolSize);
    private CardUIActionBatch currentJobBatch;

    private int currentUsingBatchCnt = 0;

    private bool bGeneratingJobBatch = false;

    public void Initialize()
    {
        for (int i = 0; i < batchPoolSize; ++i)
        {
            jobSlots.Add(jobBatchPool.Get());
            availableSlots.Enqueue(i); // 처음에 모든 인덱스를 사용 가능 상태로 넣음
        }
        InitializeCreatorMap();
    }

    private void InitializeCreatorMap()
    {
        cardLogicSystemCreatorMap = new CardLogicSystemCommandCreator[(int)CardLogicSystemEventType.MAX];
        cardDataControlSystemCreatorMap = new CardDataControlSystemCommandCreator[(int)CardDataControlSystemEventType.MAX];

        var logic = cardLogicSystemCreatorMap;

        //Card Logic System 맵 할당
        BindLogic(CardLogicSystemEventType.CardPileDrawEvent, CreateJob_Draw);
        BindLogic(CardLogicSystemEventType.CardAdditionalDrawEvent, CreateJob_AdditionalDraw);
        BindLogic(CardLogicSystemEventType.HandCardsToGraveEvent, CreateJob_HandToGrave);
        BindLogic(CardLogicSystemEventType.GraveCardsToDeckEvent, CreateJob_GraveToDeck);
        BindLogic(CardLogicSystemEventType.CardsToExtinctionEvent, CreateJob_CardsToExtinction);
        BindLogic(CardLogicSystemEventType.ExtinctionCardsToDeckEvent, CreateJob_ExtinctionToDeck);
        BindLogic(CardLogicSystemEventType.GraveCardsToHandEvent, CreateJob_GraveToHand);
        BindLogic(CardLogicSystemEventType.CardsToGraveEvent, CreateJob_CardsToGrave);
        BindLogic(CardLogicSystemEventType.CardsToHandEvent, CreateJob_CardsToHand);
        BindLogic(CardLogicSystemEventType.CardsToDeckEvent, CreateJob_CardsToDeck);

        //Card Data Control System 맵 할당
        BindData(CardDataControlSystemEventType.CardsUpgraded, CreateJob_CardsUpgraded);
        BindData(CardDataControlSystemEventType.CardsValueModified, CreateJob_CardsValueModified);

        void BindLogic(CardLogicSystemEventType type, CardLogicSystemCommandCreator action)
            => cardLogicSystemCreatorMap[(int)type] = action;

        void BindData(CardDataControlSystemEventType type, CardDataControlSystemCommandCreator action)
            => cardDataControlSystemCreatorMap[(int)type] = action;
    }

    public void ReleaseSlot(int index)
    {
        PrepareThisSlot(index); // 해당 인덱스 리스트 청소

        if (availableSlots.Contains(index)) // 이 인덱스는 이제 사용 가능.
        {
            Debug.LogWarning($"슬롯 {index}가 중복 반납되었습니다. 무시합니다.");
            return;
        }
        availableSlots.Enqueue(index);

        --currentUsingBatchCnt;

        if (currentUsingBatchCnt < 0)
            currentUsingBatchCnt = 0;
    }

    private CardUIActionBatch GetAvailableBatch()
    {
        CardUIActionBatch availableBatch;
        availableBatch.actionList = null;
        availableBatch.idx = -1;

        if (availableSlots.Count == 0)
        {
            // 만약 여유 슬롯이 없으면 새로 생성 (동적 확장)
            if (jobSlots.Count < maxBatchPoolSize)
            {
                int newIdx = jobSlots.Count;
                jobSlots.Add(jobBatchPool.Get());

                availableBatch.actionList = jobSlots[newIdx];
                availableBatch.idx = newIdx;

                return availableBatch;
            }

            Debug.LogWarning("UI 연출 명령이 포화상태입니다.");

            return availableBatch;
        }

        int slotIdx = availableSlots.Dequeue(); // 비어있는 인덱스 하나 추출

        availableBatch.actionList = jobSlots[slotIdx];
        availableBatch.idx = slotIdx;

        return availableBatch;
    }

    public void CreateCommand(CardLogicSystemEventData cardLogicSystemEventData, ReadOnlySpan<CardDataInstance> cards = default)
    {
        cardLogicSystemCreatorMap[(int)cardLogicSystemEventData.eventType]?.Invoke(cardLogicSystemEventData.contextType,cards);
    }

    public void CreateCommand(CardDataControlSystemEventData cardDataControlSystemEventData, ReadOnlySpan<CardDataInstance> cards = default)
    {
        cardDataControlSystemCreatorMap[(int)cardDataControlSystemEventData.eventType]?.Invoke(cardDataControlSystemEventData.contextType, cards);
    }

    public void CreateJob_Draw(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> drawCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var drawList = cardListPool.Get();

        for (int i = 0; i < drawCards.Length; ++i)
        {
            if (drawCards[i] != null)
                drawList.Add(drawCards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.PileDraw,
            cardSystemContextType = _cardSystemContextType,
            cards = drawList
        });
    }

    public void CreateJob_AdditionalDraw(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> drawCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var drawList = cardListPool.Get();

        for (int i = 0; i < drawCards.Length; ++i)
        {
            if (drawCards[i] != null)
                drawList.Add(drawCards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.AdditionalDraw,
            cardSystemContextType = _cardSystemContextType,
            cards = drawList
        });
    }

    public void CreateJob_HandToGrave(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> toGraveCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var drawList = cardListPool.Get();

        for (int i = 0; i < toGraveCards.Length; ++i)
        {
            if (toGraveCards[i] != null)
                drawList.Add(toGraveCards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.HandCardsToGrave,
            cardSystemContextType = _cardSystemContextType,
            cards = drawList
        });
    }

    public void CreateJob_GraveToDeck(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> toDeckCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var toDeckList = cardListPool.Get();

        for (int i = 0; i < toDeckCards.Length; ++i)
        {
            if (toDeckCards[i] != null)
                toDeckList.Add(toDeckCards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.GraveCardsToDeck,
            cardSystemContextType = _cardSystemContextType,
            cards = toDeckList
        });
    }

    public void CreateJob_GraveToHand(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> toHandCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var toHandList = cardListPool.Get();

        for (int i = 0; i < toHandCards.Length; ++i)
        {
            if (toHandCards[i] != null)
                toHandList.Add(toHandCards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.GraveCardsToHand,
            cardSystemContextType = _cardSystemContextType,
            cards = toHandList
        });
    }

    public void CreateJob_CardsToExtinction(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cardPile)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                cardList.Add(cardPile[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.CardsToExtinction,
            cardSystemContextType = _cardSystemContextType,
            cards = cardList
        });
    }

    public void CreateJob_CardsToGrave(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cardPile)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다!");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                cardList.Add(cardPile[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.CardsToGrave,
            cardSystemContextType= _cardSystemContextType,
            cards = cardList
        });
    }

    public void CreateJob_ExtinctionToDeck(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cardPile)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다.");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                cardList.Add(cardPile[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.ExtinctionCardsToDeck,
            cardSystemContextType = _cardSystemContextType,
            cards = cardList
        });
    }

    public void CreateJob_CardsToHand(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cardPile)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다.");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                cardList.Add(cardPile[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.CardsToHand,
            cardSystemContextType = _cardSystemContextType,
            cards = cardList
        });
    }

    public void CreateJob_CardsToDeck(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cardPile)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다.");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cardPile.Length; ++i)
        {
            if (cardPile[i] != null)
                cardList.Add(cardPile[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.CardsToDeck,
            cardSystemContextType = _cardSystemContextType,
            cards = cardList
        });
    }

    public void CreateJob_CardsUpgraded(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
                cardList.Add(cards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.CardsUpgraded,
            cardSystemContextType = _cardSystemContextType,
            cards = cardList
        });
    }

    public void CreateJob_CardsValueModified(CardSystemContextType _cardSystemContextType, ReadOnlySpan<CardDataInstance> cards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionList == null)
        {
            Debug.LogError("UI 명령이 포화 상태라서 연출이 누락되었습니다!");
            return;
        }

        var cardList = cardListPool.Get();

        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
                cardList.Add(cards[i]);
        }

        batch.actionList.Add(new CardUIActionData
        {
            uiActionType = CardUIActionType.CardsValueModified,
            cardSystemContextType = _cardSystemContextType,
            cards = cardList
        });
    }

    public CardUIActionBatch GetJobBatch()
    {
        if (currentJobBatch.actionList == null)
        {
            //Debug.Log("초기화되지 않은 UI명령이 배포됐습니다.");
            return default;
        }

        var resultBatch = currentJobBatch;

        currentJobBatch = default;

        bGeneratingJobBatch = false;
        ++currentUsingBatchCnt;

        return resultBatch;
    }

    private CardUIActionBatch InitializeActionDataBatch()
    {
        if (bGeneratingJobBatch == false)
        {
            currentJobBatch = GetAvailableBatch();

            if (currentJobBatch.actionList != null)
                bGeneratingJobBatch = true;
        }

        return currentJobBatch;
    }

    private void PrepareThisSlot(int index)
    {
        List<CardUIActionData> nextBatch = jobSlots[index];

        // 들어있던 내부 카드 리스트들 풀에 반납
        foreach (var job in nextBatch)
        {
            if (job.cards != null)
                cardListPool.Release(job.cards);
        }

        // 리스트 비우기 (이제 새로운 CreateJob을 받을 준비 완료)
        nextBatch.Clear();
    }
}
