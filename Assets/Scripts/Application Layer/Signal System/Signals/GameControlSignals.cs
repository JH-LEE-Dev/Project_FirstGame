
namespace GameControlSignals
{
    public struct GameStartedSignal  { }
    public struct StartSpawnWaveSignal 
    {
        public int waveIdx;

        public StartSpawnWaveSignal(int _waveIdx)
        {
            waveIdx = _waveIdx;
        }
    }

    public struct EnemyTurnStartSignal  { }
    public struct PlayerTurnStartSignal  { }
    public struct WaveStartSignal
    { 
        public int waveIdx;

        public WaveStartSignal(int _waveIdx)
        {
            waveIdx = _waveIdx;
        }
    }
}
