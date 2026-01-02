using System;
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour 
{
    public event Action StartMoveEvent;
    public event Action<uint> SpawnWaveEvent;

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
}
