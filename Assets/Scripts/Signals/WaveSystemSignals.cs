using UnityEngine;

namespace WaveSystemSignals
{
    public struct StartMoveEvent  { }
    public struct SpawnWaveEvent 
    {
        public int waveIdx;
        public SpawnWaveEvent(int idx)
        {
            waveIdx = idx;
        }
    }
    public struct WaveMoveEndEvent  { }
    public struct WaveEndEvent  { }
    public struct AllEnemyDeadEvent  { }
    public struct WaveProgressUpdatedEvent  
    {
        public Vector2 position;

        public WaveProgressUpdatedEvent(Vector2 _position)
        {
            position = _position;
        }
    }
}

