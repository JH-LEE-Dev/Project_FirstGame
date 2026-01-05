using System;
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour 
{
    [SerializeField] private float MoveTurnDelay = 2f;

    public event Action StartMoveEvent;
    public event Action<uint> SpawnWaveEvent;
    public event Action WaveMoveEndEvent;

    private WaveDatabase waveDatabase;

    public void Initialize(WaveDatabase _waveDatabase)
    {
        waveDatabase = _waveDatabase;
    }

    public void SpawnWave(int idx)
    {
        WaveData curWaveData = waveDatabase.GetWaveData(idx);

        if(curWaveData != null)
        {
            SpawnWaveEvent?.Invoke(curWaveData.enemyCnt);
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
}
