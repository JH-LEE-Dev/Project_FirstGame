using UnityEngine;

namespace WaveSystemSignals
{
    public struct StartMoveSignal  { }
    public struct SpawnWaveSignal 
    {
        public int waveIdx;
        public SpawnWaveSignal(int idx)
        {
            waveIdx = idx;
        }
    }
    public struct WaveMoveEndSignal  { }
    public struct WaveEndSignal  { }
    public struct AllEnemyDeadSignal  { }
}

