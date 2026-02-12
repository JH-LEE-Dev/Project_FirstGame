using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private IBootStrapProvider bootStrapProvider;
    private ICardLocalizationSystem cardLocalizationSystem;

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
    private GameplayUIInstaller gameplayUIInstaller;
    private SignalHub signalHub;
    private UnitSystem unitSystem;
    private CardSystem cardSystem;
    private ComplexCardEffectResolver complexCardEffectResolver;
    private CardSelectionManager cardSelectionManager;
    private ShopUIInstaller shopUIInstaller;
    private ShopSystem shopSystem;
    private ShopManager shopManager;
    private CardDataControlManager cardDataControlManager;
    private ShopBehaviorHandler shopBehaviorHandler;
    private CardFlowDataManager cardFlowDataManager;
    private ArtifactSystem artifactSystem;
    private ArtifactManager artifactManager;
    private ElementExplosionSystem elementExplosionSystem;

    [SerializeField] private WaveDatabase waveDatabase;

    public void Initialize(IBootStrapProvider _bootStrapProvider, InputManager _inputManager,
        ICardLocalizationSystem _cardLocalizationSystem)
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
        gameplayUIInstaller = GetComponentInChildren<GameplayUIInstaller>();
        unitSystem = new UnitSystem();
        cardSystem = new CardSystem();
        complexCardEffectResolver = new ComplexCardEffectResolver();
        cardSelectionManager = new CardSelectionManager();
        shopUIInstaller = GetComponentInChildren<ShopUIInstaller>();
        shopSystem = new ShopSystem();
        shopManager = GetComponent<ShopManager>();
        cardLocalizationSystem = _cardLocalizationSystem;
        cardDataControlManager = GetComponent<CardDataControlManager>();
        shopBehaviorHandler = new ShopBehaviorHandler();
        cardFlowDataManager = new CardFlowDataManager();
        artifactSystem = new ArtifactSystem();
        artifactManager = GetComponent<ArtifactManager>();
        elementExplosionSystem = GetComponent<ElementExplosionSystem>();

        gameController.Initialize(signalHub, bootStrapProvider);
        gameServiceLocator.Initialize(cameraController);

        artifactManager.Initialize();
        artifactSystem.Initialize(artifactManager, signalHub);

        elementExplosionSystem.Initialize();
        unitSystem.Initialize(signalHub, unitSpawner, unitLogicSystem, elementExplosionSystem);
        waveManager.Initialize(signalHub, waveDatabase);
        unitSpawner.Initiallize(inputManager, gameServiceLocator, environmentManager);
        unitLogicSystem.Initialize(elementExplosionSystem);

        cardFlowDataManager.Initialize();
        cardDataControlManager.Initialize();
        cardManager.Initialize();
        cardSystemController.Initialize();
        shopBehaviorHandler.Initialize(cardManager);
        cardSystem.Initialize(signalHub, cardManager, cardSystemController, cardSelectionManager, complexCardEffectResolver, cardDataControlManager,
            shopBehaviorHandler, cardFlowDataManager);

        complexCardEffectResolver.Initialize(cardManager, unitLogicSystem, cardSystemController.GetCardSlotManager(),
            cardSystemController, cardSelectionManager, cardDataControlManager, cardFlowDataManager);

        gameplayUIInstaller.Initialize(bootStrapProvider, signalHub, inputManager, cardManager, waveManager, cardLocalizationSystem, unitSpawner);
        shopUIInstaller.Initialize(bootStrapProvider, inputManager, signalHub, shopManager, cardLocalizationSystem, cardManager, unitSpawner);

        shopManager.Initialize(cardManager);
        shopSystem.Initialize(signalHub, shopManager);

        environmentManager.Initialize(unitSpawner);

        SetupGamePlayScene();
    }

    public void SetupGamePlayScene()
    {
        gameplayUIInstaller.SetupUI();
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
        gameplayUIInstaller.Release();
        shopUIInstaller.Release();
        shopSystem.Release();
        shopManager.Release();
        environmentManager.Release();
        artifactManager.Release();
        artifactSystem.Release();
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
