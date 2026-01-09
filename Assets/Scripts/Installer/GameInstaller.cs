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
    private CardEffectManager cardEffectManager;
    private UnitLogicSystem unitLogicSystem;

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
        cardEffectManager = GetComponent<CardEffectManager>();
        unitLogicSystem = GetComponent<UnitLogicSystem>();

        waveManager.Initialize(waveDatabase);
        gameServiceLocator.Initialize(cameraController, gameController, waveManager);
        gameController.Initialize(waveManager, inputManager, cardManager);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator, cardManager,cardManager,
            gameController,unitLogicSystem);
        cardEffectManager.Initialize(unitLogicSystem, cardManager);
        cardManager.Initialize(unitLogicSystem,gameController,cardUICommandSystem);

        BindEvent(cardManager, cardEffectManager);
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

    public void BindEvent(CardManager _cardManager, CardEffectManager _cardEffectManager)
    {
        cardManager.CardUsedEvent -= cardEffectManager.ExecuteCardEffect;
        cardManager.CardUsedEvent += cardEffectManager.ExecuteCardEffect;
    }

    public void ReleaseEvent()
    {
        cardManager.CardUsedEvent -= cardEffectManager.ExecuteCardEffect;
    }
}
