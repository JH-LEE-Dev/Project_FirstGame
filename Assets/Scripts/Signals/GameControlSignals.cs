
namespace GameControlSignals
{
    public struct GameStartedEvent : IPulicSignal { }
    public struct StartSpawnWaveEvent : IPulicSignal
    {
        public int waveIdx;

        public StartSpawnWaveEvent(int _waveIdx)
        {
            waveIdx = _waveIdx;
        }
    }
    public struct ActivatePlayerEvent : IPulicSignal { }

    public struct EnemyTurnStartEvent : IPulicSignal { }
    public struct PlayerTurnStartEvent : IPulicSignal { }
    public struct WaveStartEvent : IPulicSignal
    { 
        public int waveIdx;

        public WaveStartEvent(int _waveIdx)
        {
            waveIdx = _waveIdx;
        }
    }
}
