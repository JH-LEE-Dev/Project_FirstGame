using GameControlSignals;
using System;
using System.Collections;
using UnitLogicSystemSignals;
using UnityEngine;
using WaveSystemSignals;

public class WaveManager : MonoBehaviour, IWaveSystemData
{
    //외부 의존성
    SignalHub signalHub;

    [SerializeField] private float MoveTurnDelay = 2f;

    private WaveDatabase waveDatabase;

    private int currentEnemyCount = 0;
    private int maxEnemyCount = 0;
    private bool bIsWaveEnded = false;

    //인터페이스 구현 존
    public int GetCurrentWaveProgress()
    {
        return currentEnemyCount;
    }

    public int GetMaxWaveProgress()
    {
        return maxEnemyCount;
    }

    public void Initialize(SignalHub _signalHub, WaveDatabase _waveDatabase)
    {
        signalHub = _signalHub;
        waveDatabase = _waveDatabase;

        SubscribeEvents();
    }

    public void Release()
    {
        UnSubscribeEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<StartSpawnWaveEvent>(SpawnWave);
        signalHub.Subscribe<EnemyTurnStartEvent>(StartEnemyMoveTurn);
        signalHub.Subscribe<EnemyIsDeadEvent>(EnemyIsDead);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<StartSpawnWaveEvent>(SpawnWave);
        signalHub.UnSubscribe<EnemyTurnStartEvent>(StartEnemyMoveTurn);
        signalHub.UnSubscribe<EnemyIsDeadEvent>(EnemyIsDead);
    }

    private void OnDestroy()
    {

    }

    public void SpawnWave(StartSpawnWaveEvent startSpawnWaveEvent)
    {
        bIsWaveEnded = false;
        WaveData curWaveData = waveDatabase.GetWaveData(startSpawnWaveEvent.waveIdx);

        if (curWaveData != null)
        {
            currentEnemyCount = curWaveData.enemyCnt;
            maxEnemyCount = currentEnemyCount;
            signalHub.Publish(new SpawnWaveEvent(currentEnemyCount));
        }
    }

    public void StartEnemyMoveTurn(EnemyTurnStartEvent enemyTurnStartEvent)
    {
        StartCoroutine(MoveTurnCoroutine());
    }

    private IEnumerator WaveMoveEnd()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        if (bIsWaveEnded == false)
            signalHub.Publish(new WaveMoveEndEvent());
        else
            signalHub.Publish(new AllEnemyDeadEvent());
    }

    private IEnumerator MoveTurnCoroutine()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        signalHub.Publish(new StartMoveEvent());

        yield return new WaitForSeconds(MoveTurnDelay);

        StartCoroutine(WaveMoveEnd());
    }

    public void EnemyIsDead(EnemyIsDeadEvent enemyIsDeadEvent)
    {
        --currentEnemyCount;

        if (currentEnemyCount == 0)
        {
            bIsWaveEnded = true;
        }

        signalHub.Publish(new WaveProgressUpdatedEvent(enemyIsDeadEvent.position));
    }
}
