
namespace GameControlSignals
{
    public struct GameStartedEvent { }
    public struct StartSpawnWaveEvent
    {
        public int waveIdx;

        public StartSpawnWaveEvent(int _waveIdx)
        {
            waveIdx = _waveIdx;
        }
    }
    public struct ActivatePlayerEvent { }

    public struct EnemyTurnStartEvent { }
    public struct PlayerTurnStartEvent { }
    public struct WaveStartEvent 
    { 
        public int waveIdx;

        public WaveStartEvent(int _waveIdx)
        {
            waveIdx = _waveIdx;
        }
    }
}
