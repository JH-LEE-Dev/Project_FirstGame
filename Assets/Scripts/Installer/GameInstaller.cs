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

        gameServiceLocator.Initialize(cameraController, gameController);
        waveManager.Initialize(waveDatabase);
        gameController.Initialize(waveManager, inputManager, cardManager);
        unitSpawner.Initiallize(inputManager, waveManager, gameServiceLocator, cardManager,gameController);
        cardEffectManager.Initialize(unitSpawner, cardManager);

        Bind(cardManager, cardEffectManager);
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
        cardManager.CardUsedEvent -= cardEffectManager.ExecuteCardEffect;
    }

    public void DependencyInjection_Gameplay(UIInstaller uiInstaller)
    {
        uiInstaller.DependencyInjection_Gameplay(cardManager, gameController);
    }

    public void Bind(CardManager _cardManager, CardEffectManager _cardEffectManager)
    {
        cardManager.CardUsedEvent -= cardEffectManager.ExecuteCardEffect;
        cardManager.CardUsedEvent += cardEffectManager.ExecuteCardEffect;
    }
}
