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
    private CardEffectManager cardEffectManager;
    private UnitLogicSystem unitLogicSystem;

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
        cardEffectManager = GetComponent<CardEffectManager>();
        unitLogicSystem = GetComponent<UnitLogicSystem>();

        gameServiceLocator.Initialize(cameraController, gameController);
        waveManager.Initialize(waveDatabase);
        gameController.Initialize(waveManager, inputManager, cardManager);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator, cardManager,cardManager,
            gameController,unitLogicSystem);
        cardEffectManager.Initialize(unitLogicSystem, cardManager);
        cardManager.Initialize(unitLogicSystem,gameController);

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
        uiInstaller.DependencyInjection_Gameplay(cardManager,cardManager, gameController);
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
