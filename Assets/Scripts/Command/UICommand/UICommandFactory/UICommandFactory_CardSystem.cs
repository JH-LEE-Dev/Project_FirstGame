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
    private ObjectPool<List<Job_CardSystemUI>> jobBatchPool =
        new ObjectPool<List<Job_CardSystemUI>>(
            createFunc: () => new List<Job_CardSystemUI>(jobListSize),
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

    private List<List<Job_CardSystemUI>> usedBatches = new List<List<Job_CardSystemUI>>(batchPoolSize);

    private int currentUsingBatchCnt = 0;
    private int currentBatchPoolSize = batchPoolSize;
    private int writeIndex = 0;

    public void Initialize()
    {
        for (int i = 0; i < batchPoolSize; i++)
        {
            usedBatches.Add(jobBatchPool.Get());
        }
    }

    private List<Job_CardSystemUI> GetActiveBatch() => usedBatches[writeIndex];

    public void CreateJob_Draw(ReadOnlySpan<CardDataInstance> drawCards, bool bAdditional)
    {
        var batch = GetActiveBatch();

        var drawList = cardListPool.Get();

        for (int i = 0; i < drawCards.Length; ++i)
        {
            drawList.Add(drawCards[i]);
        }

        if (bAdditional == false)
            batch.Add(new Job_CardSystemUI
            {
                jobType = JobType_CardSystemUI.Draw,
                cards = drawList
            });
        else
            batch.Add(new Job_CardSystemUI
            {
                jobType = JobType_CardSystemUI.AdditionalDraw,
                cards = drawList
            });
    }

    public void CreateJob_ToGrave(ReadOnlySpan<CardDataInstance> toGraveCards)
    {
        var batch = GetActiveBatch();

        var drawList = cardListPool.Get();

        for (int i = 0; i < toGraveCards.Length; ++i)
        {
            drawList.Add(toGraveCards[i]);
        }

        batch.Add(new Job_CardSystemUI
        {
            jobType = JobType_CardSystemUI.HandToGrave,
            cards = drawList
        });
    }

    public void CreateJob_ToDeck(ReadOnlySpan<CardDataInstance> toDeckCards)
    {
        var batch = GetActiveBatch();

        var toDeckList = cardListPool.Get();

        for (int i = 0; i < toDeckCards.Length; ++i)
        {
            toDeckList.Add(toDeckCards[i]);
        }

        batch.Add(new Job_CardSystemUI
        {
            jobType = JobType_CardSystemUI.GraveToDeck,
            cards = toDeckList
        });
    }

    public List<Job_CardSystemUI> GetJobBatch()
    {
        List<Job_CardSystemUI> batchToReturn = usedBatches[writeIndex];

        ++currentUsingBatchCnt;

        if (currentUsingBatchCnt > maxBatchPoolSize)
        {
            Debug.Log("UI 연출 명령이 포화 상태입니다.");
            return null;
        }

        if (currentUsingBatchCnt > currentBatchPoolSize)
        {
            for (int i = 0; i < currentUsingBatchCnt - currentBatchPoolSize; ++i)
            {
                usedBatches.Add(jobBatchPool.Get());
            }

            currentBatchPoolSize = currentUsingBatchCnt;
        }

        writeIndex = (writeIndex + 1) % currentBatchPoolSize;

        PrepareNextSlot(writeIndex);

        return batchToReturn;
    }

    public void DecreaseBatchCount()
    {
        --currentUsingBatchCnt;

        //이런 일은 발생해서는 안됨.
        if (currentUsingBatchCnt < 0)
            currentUsingBatchCnt = 0;
    }

    private void PrepareNextSlot(int index)
    {
        List<Job_CardSystemUI> nextBatch = usedBatches[index];

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
