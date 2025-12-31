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

        unitSpawner = GetComponent<UnitSpawner>();
        waveManager = GetComponent<WaveManager>();
        gameController = GetComponent<GameController>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        deckManager = GetComponent<DeckManager>();

        gameServiceLocator.Initialize(cameraController, gameController);
        inputManager.Initialize();
        waveManager.Initialize(waveDatabase);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator);
        gameController.Initialize(waveManager, inputManager, deckManager);
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

    public void DependencyInjection(UIInstaller uiInstaller)
    {
        uiInstaller.DependencyInjection_Gameplay(deckManager);
    }
}
