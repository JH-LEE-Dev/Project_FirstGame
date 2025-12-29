using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    private InputManager inputManager;
    private UnitSpawner unitSpawner;
    private WaveManager waveManager;
    private GameController gameController;

    [SerializeField] private WaveDatabase waveDatabase;

    private void Awake()
    {
        inputManager = new InputManager();
        unitSpawner =  GetComponent<UnitSpawner>();
        waveManager = GetComponent<WaveManager>();
        gameController = GetComponent<GameController>();

        inputManager.Initialize();
        waveManager.Initialize(waveDatabase);
        unitSpawner.Initiallize(inputManager, waveManager);
        gameController.Initialize(waveManager);
    }

    private void OnDestroy()
    {
        
    }
}
