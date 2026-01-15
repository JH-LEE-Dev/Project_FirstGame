using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private IBootStrapProvider bootStrapProvider;

    //내부 의존성
    private UnitSpawner unitSpawner;
    private WaveManager waveManager;
    private GameController gameController;
    private CameraController cameraController;
    private GameServiceLocator gameServiceLocator;
    private CardManager cardManager;
    private CardEffectCommandManager cardEffectCommandManager;
    private UnitLogicSystem unitLogicSystem;
    private EnvironmentManager environmentManager;
    private GameplayUIInstaller uiInstaller;
    private SignalHub signalHub;

    [SerializeField] private WaveDatabase waveDatabase;

    public void Initialize(IBootStrapProvider _bootStrapProvider,InputManager _inputManager)
    {
        inputManager = _inputManager;

        gameController = GetComponentInChildren<GameController>();
        unitSpawner = GetComponent<UnitSpawner>();
        waveManager = GetComponentInChildren<WaveManager>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        signalHub = new SignalHub();
        cardManager = GetComponent<CardManager>();
        cardEffectCommandManager = GetComponent<CardEffectCommandManager>();
        unitLogicSystem = GetComponent<UnitLogicSystem>();
        environmentManager = GetComponentInChildren<EnvironmentManager>();
        uiInstaller = GetComponentInChildren<GameplayUIInstaller>();

        unitLogicSystem.Initialize(signalHub);
        cardEffectCommandManager.Initialize(signalHub);
        waveManager.Initialize(signalHub,waveDatabase);
        gameServiceLocator.Initialize(cameraController);
        gameController.Initialize(signalHub);
        unitSpawner.Initiallize(signalHub,inputManager,gameServiceLocator,unitLogicSystem,environmentManager);
        uiInstaller.Initialize(bootStrapProvider,signalHub, inputManager,cardManager);
        IUIModuleProvider uiModuleProvider = uiInstaller;
        cardManager.Initialize(unitLogicSystem, uiModuleProvider.uiCommandSystem, signalHub);

        SetupGamePlayScene();
    }

    public void SetupGamePlayScene()
    {
        uiInstaller.SetupUI();
        gameController.SetupGameController();
    }

    public void StartGameplayScene()
    {
        gameController.GameStart();
    }

    public void Release()
    {
        unitSpawner.Release();
        gameController.Release();
        cardManager.Release();
        cardEffectCommandManager.Release();
        unitLogicSystem.Release();
        waveManager.Release();
        uiInstaller.Release();
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
}
