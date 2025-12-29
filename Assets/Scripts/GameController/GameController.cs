using UnityEngine;

public class GameController : MonoBehaviour
{
    private WaveManager waveManager;

    public void Initialize(WaveManager _waveManager)
    {
        waveManager = _waveManager;
    }

    public void Start()
    {
        waveManager.SpawnWave(0);
    }
}
