using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class UICommandFactory_CardSystem : UICommandFactory
{
    // Job Batch Pool (Job들을 전달하기 위한 용도)
    private ObjectPool<List<Job_CardSystemUI>> jobBatchPool =
        new ObjectPool<List<Job_CardSystemUI>>(
            createFunc: () => new List<Job_CardSystemUI>(5),
            actionOnGet: (list) => list.Clear(),
            actionOnRelease: (list) => list.Clear(),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 10
        );

    // CardDataInstance 리스트 풀 (실제 카드 알맹이 담는 용도)
    private ObjectPool<List<CardDataInstance>> cardListPool =
        new ObjectPool<List<CardDataInstance>>(
            createFunc: () => new List<CardDataInstance>(20),
            actionOnGet: (list) => list.Clear(),
            actionOnRelease: (list) => list.Clear(),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 10
        );

    private List<List<Job_CardSystemUI>> usedBatches = new List<List<Job_CardSystemUI>>(5);

    private int writeIndex = 0;

    public void Initialize()
    {
        for (int i = 0; i < 5; i++)
        {
            usedBatches.Add(jobBatchPool.Get());
        }
    }

    private List<Job_CardSystemUI> GetActiveBatch() => usedBatches[writeIndex];

    public void CreateJob_Draw(ReadOnlySpan<CardDataInstance> drawCards, bool bAdditional)
    {
        var batch = GetActiveBatch();

        //if (currentSentBatch == null)
        //    batch = jobBatchPool.Get();
        //else
        //    batch = currentSentBatch;

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

        //if (currentSentBatch == null)
        //currentSentBatch = batch;
    }

    public void CreateJob_ToGrave(ReadOnlySpan<CardDataInstance> toGraveCards)
    {
        var batch = GetActiveBatch();

        //if (currentSentBatch == null)
        //    batch = jobBatchPool.Get();
        //else
        //    batch = currentSentBatch;

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

        //if (currentSentBatch == null)
        //currentSentBatch = batch;
    }

    public void CreateJob_ToDeck(ReadOnlySpan<CardDataInstance> toDeckCards)
    {
        var batch = GetActiveBatch();

        //if (currentSentBatch == null)
        //    batch = jobBatchPool.Get();
        //else
        //    batch = currentSentBatch;

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

        //if (currentSentBatch == null)
        //currentSentBatch = batch;
    }

    public List<Job_CardSystemUI> GetJobBatch()
    {
        List<Job_CardSystemUI> batchToReturn = usedBatches[writeIndex];

        writeIndex = (writeIndex + 1) % 5;

        PrepareNextSlot(writeIndex);

        return batchToReturn;
    }

    public void ReleaseJobBatch()
    {
        ++writeIndex;

        if (writeIndex >= 5)
        {
            writeIndex = 0;

            if (usedBatches[writeIndex] != null)
            {
                foreach (var oldJob in usedBatches[writeIndex])
                {
                    if (oldJob.cards != null)
                        cardListPool.Release(oldJob.cards);
                }

                jobBatchPool.Release(usedBatches[writeIndex]);
                //usedBatches[usedBatchCnt] = null;
            }

            usedBatches[writeIndex] = jobBatchPool.Get();
        }

        //lastSentBatch = currentSentBatch;
        //currentSentBatch = null;
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
