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

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;

        gameController = GetComponent<GameController>();
        unitSpawner = GetComponent<UnitSpawner>();
        waveManager = GetComponent<WaveManager>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        deckManager = GetComponent<DeckManager>();

        gameServiceLocator.Initialize(cameraController, gameController);
        waveManager.Initialize(waveDatabase);
        gameController.Initialize(waveManager, inputManager, deckManager);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator, deckManager,gameController);
    }

    private void Awake()
    {

    }

    private void Start()
    {
        //Sound.PlayBGM("BGM");    
    }

    private void OnDestroy()
    {

    }

    public void DependencyInjection_Gameplay(UIInstaller uiInstaller)
    {
        uiInstaller.DependencyInjection_Gameplay(deckManager, gameController);
    }
}
