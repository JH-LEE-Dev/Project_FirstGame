using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataBase", menuName = "Game/Wave Database")]
public class WaveDatabase : ScriptableObject
{
    public List<WaveData> waveData;

    public WaveData GetWaveData(int idx)
    {
        return waveData[idx];
    }
}