using System;
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour, IWaveSystemActions, IWaveSystemEvents
{
    [SerializeField] private float MoveTurnDelay = 2f;

    public event Action StartMoveEvent;
    public event Action<uint> SpawnWaveEvent;
    public event Action WaveMoveEndEvent;
    public event Action WaveEndEvent;

    private WaveDatabase waveDatabase;

    private uint currentEnemyCount = 0;

    public void Initialize(WaveDatabase _waveDatabase)
    {
        waveDatabase = _waveDatabase;
    }

    private void OnDestroy()
    {
        StartMoveEvent = null;
        SpawnWaveEvent = null;
        WaveMoveEndEvent = null;
        WaveEndEvent = null;
    }

    public void SpawnWave(int idx)
    {
        WaveData curWaveData = waveDatabase.GetWaveData(idx);

        if (curWaveData != null)
        {
            currentEnemyCount = curWaveData.enemyCnt;
            SpawnWaveEvent?.Invoke(currentEnemyCount);
        }
    }

    public void StartEnemyMoveTurn()
    {
        StartCoroutine(MoveTurnCoroutine());
    }

    private IEnumerator WaveMoveEnd()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        WaveMoveEndEvent?.Invoke();
    }

    private IEnumerator MoveTurnCoroutine()
    {
        yield return new WaitForSeconds(MoveTurnDelay);

        StartMoveEvent?.Invoke();

        yield return new WaitForSeconds(MoveTurnDelay);

        StartCoroutine(WaveMoveEnd());
    }

    public void EnemyIsDead()
    {
        --currentEnemyCount;

        if (currentEnemyCount == 0)
        {
            WaveEndEvent?.Invoke();
        }
    }
}
