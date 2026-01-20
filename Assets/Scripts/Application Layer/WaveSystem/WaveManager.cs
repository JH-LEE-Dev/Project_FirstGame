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
        signalHub.Subscribe<StartSpawnWaveSignal>(SpawnWave);
        signalHub.Subscribe<EnemyTurnStartSignal>(StartEnemyMoveTurn);
        signalHub.Subscribe<EnemyIsDeadSignal>(EnemyIsDead);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<StartSpawnWaveSignal>(SpawnWave);
        signalHub.UnSubscribe<EnemyTurnStartSignal>(StartEnemyMoveTurn);
        signalHub.UnSubscribe<EnemyIsDeadSignal>(EnemyIsDead);
    }

    private void OnDestroy()
    {

    }

    public void SpawnWave(StartSpawnWaveSignal startSpawnWaveSignal)
    {
        bIsWaveEnded = false;
        WaveData curWaveData = waveDatabase.GetWaveData(startSpawnWaveSignal.waveIdx);

        if (curWaveData != null)
        {
            currentEnemyCount = curWaveData.enemyCnt;
            maxEnemyCount = currentEnemyCount;
            signalHub.Publish(new SpawnWaveSignal(currentEnemyCount));
        }
    }

    public void StartEnemyMoveTurn(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        StartCoroutine(MoveTurnCoroutine());
    }

    private IEnumerator WaveMoveEnd()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        if (bIsWaveEnded == false)
            signalHub.Publish(new WaveMoveEndSignal());
        else
            signalHub.Publish(new AllEnemyDeadSignal());
    }

    private IEnumerator MoveTurnCoroutine()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        signalHub.Publish(new StartMoveSignal());

        yield return new WaitForSeconds(MoveTurnDelay);

        StartCoroutine(WaveMoveEnd());
    }

    public void EnemyIsDead(EnemyIsDeadSignal enemyIsDeadSignal)
    {
        --currentEnemyCount;

        if (currentEnemyCount == 0)
        {
            bIsWaveEnded = true;
        }

        signalHub.Publish(new WaveProgressUpdatedSignal(enemyIsDeadSignal.position));
    }
}
