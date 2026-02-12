using UnityEngine;

namespace WaveSystemSignals
{
    public struct StartMoveSignal  { }
    public struct SpawnWaveSignal 
    {
        public int waveIdx;
        public bool bAdditional;
        public SpawnWaveSignal(int idx,bool _boolean)
        {
            waveIdx = idx;
            bAdditional = _boolean;
        }
    }
    public struct WaveMoveEndSignal  { }
    public struct WaveEndSignal  { }
    public struct AllEnemyDeadSignal  { }
    public struct WaveCompleteRewardSignal
    {
        public int moneyAmount;
        public WaveCompleteRewardSignal(int _moneyAmount)
        {
            moneyAmount = _moneyAmount;
        }
    }
}

