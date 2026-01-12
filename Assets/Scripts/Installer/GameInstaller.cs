using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private ICardUICommandSystem cardUICommandSystem;

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

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;

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
        gameServiceLocator.Initialize(cameraController, gameController, waveManager);
        gameController.Initialize(waveManager, inputManager, cardManager);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator, cardManager,cardManager,
            gameController,unitLogicSystem,environmentManager);
        cardManager.Initialize(unitLogicSystem,gameController,cardUICommandSystem);

        BindEvent(cardManager,unitLogicSystem, cardEffectCommandManager);
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

    public void DependencyInjection_Gameplay(UIInstaller uiInstaller)
    {
        uiInstaller.ReceiveDependency_Gameplay(cardManager,cardManager, gameController,unitLogicSystem);
    }

    public void ReceiveDependency_Gameplay(ICardUICommandSystem _cardUICommandSystem)
    {
        cardUICommandSystem = _cardUICommandSystem;
    }

    public void BindEvent(CardManager _cardManager, UnitLogicSystem _unitLogicSystem,CardEffectCommandManager _cardEffectManager)
    {
        cardManager.CardUsedEvent -= cardEffectCommandManager.AnalysisCardEffect;
        cardManager.CardUsedEvent += cardEffectCommandManager.AnalysisCardEffect;

        cardEffectCommandManager.CardEffectStatusCommandDispatchEvent -= unitLogicSystem.InsertCommand;
        cardEffectCommandManager.CardEffectStatusCommandDispatchEvent += unitLogicSystem.InsertCommand;
        cardEffectCommandManager.CardEffectSystemCommandDispatchEvent -= cardManager.InsertCommand;
        cardEffectCommandManager.CardEffectSystemCommandDispatchEvent += cardManager.InsertCommand;
    }

    public void ReleaseEvent()
    {
        cardManager.CardUsedEvent -= cardEffectCommandManager.AnalysisCardEffect;

        cardEffectCommandManager.CardEffectStatusCommandDispatchEvent -= unitLogicSystem.InsertCommand;
        cardEffectCommandManager.CardEffectSystemCommandDispatchEvent -= cardManager.InsertCommand;
    }
}
