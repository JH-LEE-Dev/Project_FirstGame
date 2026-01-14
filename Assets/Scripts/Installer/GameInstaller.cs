using UnityEngine;

public class GameInstaller : MonoBehaviour,IGameModuleProvider
{
    public ICardSystemEvents cardSystemEvents => cardManager;

    public ICardSystemData cardSystemData => cardManager;

    public ICardSystemActions cardSystemActions => cardManager;

    public IUnitLogicSystemData unitLogicSystemData => unitLogicSystem;

    public IUnitEventAccessor unitEventAccessor => unitSpawner;

    public IUnitSpawnSystemEvent unitSpawnSystemEvent => unitSpawner;

    //외부 의존성
    private InputManager inputManager;
    private ICardUICommandSystem cardUICommandSystem;
    private IUIModuleProvider uiModuleProvider;
    private IUISignalHubProvider uiSignalHubProvider;

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

    [SerializeField] private WaveDatabase waveDatabase;

    public void Initialize(InputManager _inputManager,IUISignalHubProvider _uiSignalHubProvider,IUIModuleProvider _uiModuleProvider)
    {
        inputManager = _inputManager;
        uiModuleProvider = _uiModuleProvider;
        uiSignalHubProvider = _uiSignalHubProvider;
        cardUICommandSystem = uiModuleProvider.uiCommandSystem;

        gameController = GetComponentInChildren<GameController>();
        unitSpawner = GetComponent<UnitSpawner>();
        waveManager = GetComponentInChildren<WaveManager>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        cardManager = GetComponent<CardManager>();
        cardEffectCommandManager = GetComponent<CardEffectCommandManager>();
        unitLogicSystem = GetComponent<UnitLogicSystem>();
        environmentManager = GetComponentInChildren<EnvironmentManager>();

        waveManager.Initialize(waveDatabase);
        gameServiceLocator.Initialize(cameraController);
        gameController.Initialize(waveManager, inputManager, cardManager, uiSignalHubProvider,unitSpawner);
        unitSpawner.Initiallize(inputManager, waveManager,waveManager, gameServiceLocator, cardManager,cardManager,
            gameController,unitLogicSystem,environmentManager);
        cardManager.Initialize(unitLogicSystem,gameController,cardUICommandSystem);

        BindEvent();
    }

    public void Release()
    {
        unitSpawner.Release();
        gameController.Release();
        ReleaseEvent();
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
        ReleaseEvent();
    }

    public void BindEvent()
    {
        cardManager.CardUsedEvent -= cardEffectCommandManager.AnalysisCardEffect;
        cardManager.CardUsedEvent += cardEffectCommandManager.AnalysisCardEffect;

        cardEffectCommandManager.CardEffectStatusCommandDispatchEvent -= unitLogicSystem.InsertCommand;
        cardEffectCommandManager.CardEffectStatusCommandDispatchEvent += unitLogicSystem.InsertCommand;
        cardEffectCommandManager.CardEffectSystemCommandDispatchEvent -= cardManager.InsertCommand;
        cardEffectCommandManager.CardEffectSystemCommandDispatchEvent += cardManager.InsertCommand;

        gameController.SpawnWaveEvent -= waveManager.SpawnWave;
        gameController.SpawnWaveEvent += waveManager.SpawnWave;
        gameController.SpawnPlayerEvent -= unitSpawner.SpawnPlayerAndCharacter;
        gameController.SpawnPlayerEvent += unitSpawner.SpawnPlayerAndCharacter;
        waveManager.SpawnWaveEvent -= unitSpawner.SpawnWave;
        waveManager.SpawnWaveEvent += unitSpawner.SpawnWave;
        waveManager.WaveEndEvent -= unitSpawner.ResetCurrentEnemies;
        waveManager.WaveEndEvent += unitSpawner.ResetCurrentEnemies;
        waveManager.WaveMoveEndEvent -= gameController.ChangeGameStateToPlayerTurn;
        waveManager.WaveMoveEndEvent += gameController.ChangeGameStateToPlayerTurn;
    }

    public void ReleaseEvent()
    {
        cardManager.CardUsedEvent -= cardEffectCommandManager.AnalysisCardEffect;
        gameController.SpawnWaveEvent -= waveManager.SpawnWave;
        gameController.SpawnPlayerEvent -= unitSpawner.SpawnPlayerAndCharacter;
        waveManager.SpawnWaveEvent -= unitSpawner.SpawnWave;
        waveManager.WaveEndEvent -= unitSpawner.ResetCurrentEnemies;
        waveManager.WaveMoveEndEvent -= gameController.ChangeGameStateToPlayerTurn;
        cardEffectCommandManager.CardEffectStatusCommandDispatchEvent -= unitLogicSystem.InsertCommand;
        cardEffectCommandManager.CardEffectSystemCommandDispatchEvent -= cardManager.InsertCommand;
    }
}
