using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class UICommandFactory_CardSystem : UICommandFactory
{
    // 현재 UI가 사용 중인 Batch를 시스템이 보관
    private List<Job_CardSystemUI> lastSentBatch;
    private List<Job_CardSystemUI> currentSentBatch;

    // Job Batch Pool (Job들을 전달하기 위한 용도)
    private ObjectPool<List<Job_CardSystemUI>> jobBatchPool =
        new ObjectPool<List<Job_CardSystemUI>>(
            createFunc: () => new List<Job_CardSystemUI>(5),
            actionOnGet: (list) => list.Clear(),
            actionOnRelease: (list) => list.Clear(),
            collectionCheck: true,
            defaultCapacity: 5,
            maxSize: 10
        );

    // CardDataInstance 리스트 풀 (실제 카드 알맹이 담는 용도)
    private ObjectPool<List<CardDataInstance>> cardListPool =
        new ObjectPool<List<CardDataInstance>>(
            createFunc: () => new List<CardDataInstance>(20),
            actionOnGet: (list) => list.Clear(),
            actionOnRelease: (list) => list.Clear(),
            collectionCheck: true,
            defaultCapacity: 5,
            maxSize: 10
        );

    public void CreateJob_Draw(ReadOnlySpan<CardDataInstance> drawCards)
    {
        var batch = jobBatchPool.Get();
        var drawList = cardListPool.Get();

        for (int i = 0; i < drawCards.Length; ++i)
        {
            drawList.Add(drawCards[i]);
        }

        batch.Add(new Job_CardSystemUI
        {
            jobType = JobType_CardSystemUI.Draw,
            cards = drawList
        });

        currentSentBatch = batch;   
    }

    public List<Job_CardSystemUI> GetJobBatch()
    {
        ReleaseJobBatch();

        return lastSentBatch;
    }

    public void ReleaseJobBatch()
    {
        if (lastSentBatch != null)
        {
            foreach (var oldJob in lastSentBatch)
            {
                if (oldJob.cards != null)
                    cardListPool.Release(oldJob.cards);
            }

            jobBatchPool.Release(lastSentBatch);
            lastSentBatch = null;
        }

        lastSentBatch = currentSentBatch;
    }
}
