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
        PrepareNextSlot(index); // 해당 인덱스 리스트 청소
        availableSlots.Enqueue(index); // 이제 다시 써도 된다고 큐에 삽입
        --currentUsingBatchCnt;

        if (currentUsingBatchCnt < 0)
            currentUsingBatchCnt = 0;
    }

    private ActionDataBatch_CardSystem GetAvailableBatch()
    {
        ActionDataBatch_CardSystem availableBatch;
        availableBatch.actionDataList = null;
        availableBatch.idx = 0;

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
            return;

        var drawList = cardListPool.Get();

        for (int i = 0; i < drawCards.Length; ++i)
        {
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
            return;

        var drawList = cardListPool.Get();

        for (int i = 0; i < toGraveCards.Length; ++i)
        {
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
            return;

        var toDeckList = cardListPool.Get();

        for (int i = 0; i < toDeckCards.Length; ++i)
        {
            toDeckList.Add(toDeckCards[i]);
        }

        batch.actionDataList.Add(new ActionData_CardSystem
        {
            actionDataType = ActionType_CardSystem.GraveToDeck,
            cards = toDeckList
        });
    }

    public ActionDataBatch_CardSystem GetJobBatch()
    {
        bGeneratingJobBatch = false;
        ++currentUsingBatchCnt;

        return currentJobBatch;
    }

    private ActionDataBatch_CardSystem InitializeActionDataBatch()
    {
        ActionDataBatch_CardSystem batch;

        if (bGeneratingJobBatch)
            batch = currentJobBatch;
        else
        {
            currentJobBatch = GetAvailableBatch();
            batch = currentJobBatch;

            if (batch.actionDataList != null)
                bGeneratingJobBatch = true;
        }

        return batch;
    }

    private void PrepareNextSlot(int index)
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
