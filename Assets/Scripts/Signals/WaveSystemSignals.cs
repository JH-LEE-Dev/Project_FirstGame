using UnityEngine;

namespace WaveSystemSignals
{
    public struct StartMoveEvent : IPulicSignal { }
    public struct SpawnWaveEvent : IPulicSignal
    {
        public int waveIdx;
        public SpawnWaveEvent(int idx)
        {
            waveIdx = idx;
        }
    }
    public struct WaveMoveEndEvent : IPulicSignal { }
    public struct WaveEndEvent : IPulicSignal { }
    public struct AllEnemyDeadEvent : IPulicSignal { }
    public struct WaveProgressUpdatedEvent : IPulicSignal 
    {
        public Vector2 position;

        public WaveProgressUpdatedEvent(Vector2 _position)
        {
            position = _position;
        }
    }
}

