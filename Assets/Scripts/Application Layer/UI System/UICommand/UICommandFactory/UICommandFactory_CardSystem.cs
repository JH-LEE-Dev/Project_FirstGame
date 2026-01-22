using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class UICommandFactory_CardSystem : UICommandFactory
{
    const int maxBatchPoolSize = 10;
    const int jobListSize = 10;
    const int cardListSize = 30;
    const int batchPoolSize = 5;

    // Job Batch Pool (Job들을 전달하기 위한 용도)
    private ObjectPool<List<ActionData_CardSystem>> jobBatchPool =
        new ObjectPool<List<ActionData_CardSystem>>(
            createFunc: () => new List<ActionData_CardSystem>(jobListSize),
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
    private List<List<ActionData_CardSystem>> jobSlots = new List<List<ActionData_CardSystem>>(maxBatchPoolSize);
    private ActionDataBatch_CardSystem currentJobBatch;

    private int currentUsingBatchCnt = 0;

    private bool bGeneratingJobBatch = false;

    public void Initialize()
    {
        for (int i = 0; i < batchPoolSize; ++i)
        {
            jobSlots.Add(jobBatchPool.Get());
            availableSlots.Enqueue(i); // 처음에 모든 인덱스를 사용 가능 상태로 넣음
        }
    }

    public void ReleaseSlot(int index)
    {
        PrepareThisSlot(index); // 해당 인덱스 리스트 청소

        if (availableSlots.Contains(index)) // 이 인덱스는 이제 사용 가능.
        {
            Debug.LogWarning($"슬롯 {index}가 중복 반납되었습니다. 무시합니다.");
            return;
        }

        --currentUsingBatchCnt;

        if (currentUsingBatchCnt < 0)
            currentUsingBatchCnt = 0;
    }

    private ActionDataBatch_CardSystem GetAvailableBatch()
    {
        ActionDataBatch_CardSystem availableBatch;
        availableBatch.actionDataList = null;
        availableBatch.idx = -1;

        if (availableSlots.Count == 0)
        {
            // 만약 여유 슬롯이 없으면 새로 생성 (동적 확장)
            if (jobSlots.Count < maxBatchPoolSize)
            {
                int newIdx = jobSlots.Count;
                jobSlots.Add(jobBatchPool.Get());

                availableBatch.actionDataList = jobSlots[newIdx];
                availableBatch.idx = newIdx;

                return availableBatch;
            }

            Debug.Log("UI 연출 명령이 포화상태입니다.");

            return availableBatch;
        }

        int slotIdx = availableSlots.Dequeue(); // 비어있는 인덱스 하나 추출

        availableBatch.actionDataList = jobSlots[slotIdx];
        availableBatch.idx = slotIdx;

        return availableBatch;
    }

    public void CreateJob_Draw(ReadOnlySpan<CardDataInstance> drawCards, bool bAdditional)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
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

        if (bAdditional == false)
            batch.actionDataList.Add(new ActionData_CardSystem
            {
                actionDataType = ActionType_CardSystem.PileDraw,
                cards = drawList
            });
        else
            batch.actionDataList.Add(new ActionData_CardSystem
            {
                actionDataType = ActionType_CardSystem.AdditionalDraw,
                cards = drawList
            });
    }

    public void CreateJob_ToGrave(ReadOnlySpan<CardDataInstance> toGraveCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
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

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.HandToGrave,
            cards = drawList
        });
    }

    public void CreateJob_ToDeck(ReadOnlySpan<CardDataInstance> toDeckCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
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

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.GraveToDeck,
            cards = toDeckList
        });
    }

    public void CreateJob_GraveToHand(ReadOnlySpan<CardDataInstance> toHandCards)
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
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

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.GraveToHand,
            cards = toHandList
        });
    }

    public void CreateJob_ToExtinction()
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다!");
            return;
        }

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.UsedCardToExtinction,
            cards = null
        });
    }

    public void CreateJob_UsedCardToGrave()
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다!");
            return;
        }

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.UsedCardToGrave,
            cards = null
        });
    }

    public void CreateJob_ExtinctionToDeck()
    {
        var batch = InitializeActionDataBatch();
        if (batch.actionDataList == null)
        {
            Debug.LogError("UI 명령 풀이 가득 차서 연출이 누락되었습니다.");
            return;
        }

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.ExtinctionToDeck,
            cards = null
        });
    }

    public ActionDataBatch_CardSystem GetJobBatch()
    {
        if(currentJobBatch.actionDataList == null)
        {
            Debug.Log("초기화되지 않은 UI명령이 배포됐습니다.");
            return default;
        }

        var resultBatch = currentJobBatch;

        currentJobBatch = default;

        bGeneratingJobBatch = false;
        ++currentUsingBatchCnt;

        return resultBatch;
    }

    private ActionDataBatch_CardSystem InitializeActionDataBatch()
    {
        if (bGeneratingJobBatch == false)
        {
            currentJobBatch = GetAvailableBatch();

            if (currentJobBatch.actionDataList != null)
                bGeneratingJobBatch = true;
        }

        return currentJobBatch;
    }

    private void PrepareThisSlot(int index)
    {
        List<ActionData_CardSystem> nextBatch = jobSlots[index];

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
