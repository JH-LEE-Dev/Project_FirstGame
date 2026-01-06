using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    private InputManager inputManager;
    private UnitSpawner unitSpawner;
    private WaveManager waveManager;
    private GameController gameController;
    private CameraController cameraController;
    private GameServiceLocator gameServiceLocator;
    private CardManager cardManager;

    [SerializeField] private WaveDatabase waveDatabase;

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;

        gameController = GetComponent<GameController>();
        unitSpawner = GetComponent<UnitSpawner>();
        waveManager = GetComponent<WaveManager>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        cardManager = GetComponent<CardManager>();

        gameServiceLocator.Initialize(cameraController, gameController);
        waveManager.Initialize(waveDatabase);
        gameController.Initialize(waveManager, inputManager, cardManager);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator, cardManager,gameController);
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
        uiInstaller.DependencyInjection_Gameplay(cardManager, gameController);
    }
}
