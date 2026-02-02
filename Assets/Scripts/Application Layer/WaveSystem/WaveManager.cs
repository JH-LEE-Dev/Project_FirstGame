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
    private int remainkilledEnemyCnt = 0;
    private int numberOfEnemiesToKill = 0;
    private int spawnEnemyCnt = 0;
    private int currentEnemyThreshold = 0;
    private bool bIsWaveEnded = false;

    //인터페이스 구현 존
    public int GetCurrentWaveProgress()
    {
        return remainkilledEnemyCnt;
    }

    public int GetMaxWaveProgress()
    {
        return numberOfEnemiesToKill;
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
        signalHub.Subscribe<EnemyIsKilledSignal>(EnemyIsKilled);
        signalHub.Subscribe<EnemyIsDeadSignal>(EnemyIsDead);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<StartSpawnWaveSignal>(SpawnWave);
        signalHub.UnSubscribe<EnemyTurnStartSignal>(StartEnemyMoveTurn);
        signalHub.UnSubscribe<EnemyIsKilledSignal>(EnemyIsKilled);
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
            currentEnemyCount = curWaveData.InitialEnemyCnt;
            numberOfEnemiesToKill = curWaveData.numberOfEnemiesToKill;
            remainkilledEnemyCnt = curWaveData.numberOfEnemiesToKill;
            currentEnemyThreshold = curWaveData.currentEnemyThreshold;
            spawnEnemyCnt = curWaveData.spawnEnemyCnt;
            signalHub.Publish(new SpawnWaveSignal(currentEnemyCount));
        }
    }

    public void SpawnAdditionalWave()
    {
        signalHub.Publish(new SpawnWaveSignal(spawnEnemyCnt));
    }

    public void StartEnemyMoveTurn(EnemyTurnStartSignal enemyTurnStartSignal)
    {
        if (bIsWaveEnded == false)
            StartCoroutine(MoveTurnCoroutine());
        else
            signalHub.Publish(new AllEnemyDeadSignal());
    }

    private IEnumerator WaveMoveEnd()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        signalHub.Publish(new WaveMoveEndSignal());
    }

    private IEnumerator MoveTurnCoroutine()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        signalHub.Publish(new StartMoveSignal());

        yield return new WaitForSeconds(MoveTurnDelay);

        StartCoroutine(WaveMoveEnd());
    }

    public void EnemyIsKilled(EnemyIsKilledSignal enemyIskilledSignal)
    {
        --currentEnemyCount;
        --remainkilledEnemyCnt;

        signalHub.Publish(new WaveProgressUpdatedSignal(enemyIskilledSignal.enemyData));

        if (remainkilledEnemyCnt == 0)
        {
            bIsWaveEnded = true;
            return;
        }

        if (currentEnemyCount <= currentEnemyThreshold)
        {
            SpawnAdditionalWave();
        }
    }

    private void EnemyIsDead(EnemyIsDeadSignal enemyIsDeadSignal)
    {
        --currentEnemyCount;

        if (currentEnemyCount <= currentEnemyThreshold)
        {
            SpawnAdditionalWave();
        }
    }
}
