using UnityEngine;
using UnityEngine.U2D;

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
    private CardSystemController cardSystemController;
    private UnitLogicSystem unitLogicSystem;
    private EnvironmentManager environmentManager;
    private GameplayUIInstaller uiInstaller;
    private SignalHub signalHub;
    private UnitSystem unitSystem;
    private CardSystem cardSystem;
    private ComplexCardEffectResolver complexCardEffectResolver;
    private CardSelectionManager cardSelectionManager;
    private ShopUIInstaller shopUIInstaller;
    private ShopSystem shopSystem;
    private ShopManager shopManager;

    [SerializeField] private WaveDatabase waveDatabase;

    public void Initialize(IBootStrapProvider _bootStrapProvider, InputManager _inputManager)
    {
        inputManager = _inputManager;
        bootStrapProvider = _bootStrapProvider;

        gameController = GetComponentInChildren<GameController>();
        unitSpawner = GetComponent<UnitSpawner>();
        waveManager = GetComponentInChildren<WaveManager>();
        cameraController = GetComponent<CameraController>();
        gameServiceLocator = new GameServiceLocator();
        signalHub = new SignalHub();
        cardManager = GetComponent<CardManager>();
        cardSystemController = GetComponent<CardSystemController>();
        unitLogicSystem = GetComponent<UnitLogicSystem>();
        environmentManager = GetComponentInChildren<EnvironmentManager>();
        uiInstaller = GetComponentInChildren<GameplayUIInstaller>();
        unitSystem = new UnitSystem();
        cardSystem = new CardSystem();
        complexCardEffectResolver = new ComplexCardEffectResolver();
        cardSelectionManager= new CardSelectionManager();
        shopUIInstaller = GetComponentInChildren<ShopUIInstaller>();
        shopSystem = new ShopSystem();
        shopManager = GetComponent<ShopManager>();

        gameController.Initialize(signalHub);
        gameServiceLocator.Initialize(cameraController);

        unitSystem.Initialize(signalHub, unitSpawner, unitLogicSystem);
        waveManager.Initialize(signalHub, waveDatabase);
        unitSpawner.Initiallize(inputManager, gameServiceLocator, environmentManager);
        unitLogicSystem.Initialize();

        cardManager.Initialize();
        cardSystemController.Initialize();
        cardSystem.Initialize(signalHub, cardManager, cardSystemController,cardSelectionManager, complexCardEffectResolver);

        complexCardEffectResolver.Initialize(cardManager, unitLogicSystem, cardSystemController.GetCardSlotManager(), cardSystemController,cardSelectionManager);

        uiInstaller.Initialize(bootStrapProvider, signalHub, inputManager, cardManager, waveManager);
        shopUIInstaller.Initialize(bootStrapProvider, inputManager,signalHub,shopManager);

        shopSystem.Initialize(signalHub, shopManager);

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
        cardSystem.Release();
        unitSystem.Release();
        unitSpawner.Release();
        unitLogicSystem.Release();
        gameController.Release();
        cardManager.Release();
        cardSystemController.Release();
        waveManager.Release();
        uiInstaller.Release();
        shopUIInstaller.Release();
        shopSystem.Release();
        shopManager.Release();
        environmentManager.Release();
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
