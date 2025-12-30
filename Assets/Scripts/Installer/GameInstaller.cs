using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    private InputManager inputManager;
    private UnitSpawner unitSpawner;
    private WaveManager waveManager;
    private GameController gameController;
    private CameraController cameraController;
    private GameServiceLocator gameServiceLocator;
    private DeckManager deckManager;

    [SerializeField] private WaveDatabase waveDatabase;

    private void Awake()
    {
        inputManager = new InputManager();
        unitSpawner =  GetComponent<UnitSpawner>();
        waveManager = GetComponent<WaveManager>();
        gameController = GetComponent<GameController>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        deckManager = new DeckManager();

        gameServiceLocator.Initialize(cameraController,gameController);
        inputManager.Initialize();
        waveManager.Initialize(waveDatabase);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator);
        gameController.Initialize(waveManager, inputManager,deckManager);
    }

    private void Start()
    {
        Sound.PlayBGM("BGM");    
    }

    private void OnDestroy()
    {
        
    }
}
