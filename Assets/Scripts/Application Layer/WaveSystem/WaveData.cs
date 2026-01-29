using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class WaveData
{
    public int waveIdx = 0;
    public int spawnEnemyCnt = 0;
    public int InitialEnemyCnt = 0;
    public int currentEnemyThreshold = 0;
    public int numberOfEnemiesToKill = 0;
}