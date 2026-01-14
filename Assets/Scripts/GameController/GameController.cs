using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameController : MonoBehaviour, IGameFlowController
{
    public event Action<int> SpawnWaveEvent;
    public event Action SpawnPlayerEvent;

    //외부 의존성.
    private InputManager inputManager;
    private ICardSystemFlowActions cardSystemFlowActions;
    private IWaveSystemActions waveSystemActions;
    private IUISignalHubProvider uiSignalHubProvider;
    private IUnitSignalHubProvider unitSignalHubProvider;

    //내부 의존성
    private GameStateMachine gameStateMachine;


    private int waveIdx = 0;

    public void Initialize(IWaveSystemActions _waveSystemActions, InputManager _inputManager,
        ICardSystemFlowActions _cardSystemFlowActions,IUISignalHubProvider _uiHubProvider,
        IUnitSignalHubProvider _unitSignalProvider)
    {
        waveSystemActions = _waveSystemActions;
        inputManager = _inputManager;
        cardSystemFlowActions = _cardSystemFlowActions;
        uiSignalHubProvider = _uiHubProvider;
        unitSignalHubProvider = _unitSignalProvider;
        gameStateMachine = new GameStateMachine();

        SetGameState();
    }

    private void SetGameState()
    {
        GS_PlayerTurnState playerTurn = new GS_PlayerTurnState();
        playerTurn.Initialize(gameStateMachine);
        gameStateMachine.AddState(playerTurn);

        GS_EnemyTurnState enemyTurn = new GS_EnemyTurnState();
        enemyTurn.Initialize(gameStateMachine);
        gameStateMachine.AddState(enemyTurn);

        GS_GameStarted gameStarted = new GS_GameStarted();
        gameStarted.Initialize(gameStateMachine);
        gameStateMachine.AddState(gameStarted);

        BindEvent(enemyTurn, playerTurn, gameStarted);
    }

    public void BindEvent(GS_EnemyTurnState enemyTurn, GS_PlayerTurnState playerTurn,GS_GameStarted gameStarted)
    {
        gameStarted.SpawnUnitsEvent -= SpawnUnits;
        gameStarted.SpawnUnitsEvent += SpawnUnits;

        enemyTurn.EnemyTurnStartEvent -= waveSystemActions.StartEnemyMoveTurn;
        enemyTurn.EnemyTurnStartEvent += waveSystemActions.StartEnemyMoveTurn;

        enemyTurn.EnemyTurnStartEvent -= uiSignalHubProvider.cardUISignalHub.EnemyTurnStarted;
        enemyTurn.EnemyTurnStartEvent += uiSignalHubProvider.cardUISignalHub.EnemyTurnStarted;

        enemyTurn.EnemyTurnStartEvent -= uiSignalHubProvider.gameplayUISignalHub.EnemyTurnStarted;
        enemyTurn.EnemyTurnStartEvent += uiSignalHubProvider.gameplayUISignalHub.EnemyTurnStarted;

        playerTurn.PlayerTurnStartEvent -= cardSystemFlowActions.StartCardDrawTurn;
        playerTurn.PlayerTurnStartEvent += cardSystemFlowActions.StartCardDrawTurn;

        playerTurn.PlayerTurnStartEvent -= uiSignalHubProvider.cardUISignalHub.PlayerTurnStarted;
        playerTurn.PlayerTurnStartEvent += uiSignalHubProvider.cardUISignalHub.PlayerTurnStarted;

        playerTurn.PlayerTurnStartEvent -= uiSignalHubProvider.gameplayUISignalHub.PlayerTurnStarted;
        playerTurn.PlayerTurnStartEvent += uiSignalHubProvider.gameplayUISignalHub.PlayerTurnStarted;
    }

    public void BindCharacter()
    {
        GS_EnemyTurnState enemyTurnState = gameStateMachine.GetState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= unitSignalHubProvider.characterSignalHub.ResetbCanAction;
            enemyTurnState.EnemyTurnStartEvent += unitSignalHubProvider.characterSignalHub.ResetbCanAction;
        }
    }

    public void ReleaseCharacter()
    {
        GS_EnemyTurnState enemyTurnState = gameStateMachine.GetState<GS_EnemyTurnState>();

        if (enemyTurnState != null)
        {
            enemyTurnState.EnemyTurnStartEvent -= unitSignalHubProvider.characterSignalHub.ResetbCanAction;
        }
    }

    public void BindEnemy()
    {

    }

    public void ReleaseEvent()
    {
        GS_PlayerTurnState playerTurn = gameStateMachine.GetState<GS_PlayerTurnState>();
        GS_EnemyTurnState enemyTurn = gameStateMachine.GetState<GS_EnemyTurnState>();
        GS_GameStarted gameStarted = gameStateMachine.GetState<GS_GameStarted>();

        gameStarted.SpawnUnitsEvent -= SpawnUnits;

        enemyTurn.EnemyTurnStartEvent -= waveSystemActions.StartEnemyMoveTurn;
        enemyTurn.EnemyTurnStartEvent -= uiSignalHubProvider.cardUISignalHub.EnemyTurnStarted;
        enemyTurn.EnemyTurnStartEvent -= uiSignalHubProvider.gameplayUISignalHub.EnemyTurnStarted;

        playerTurn.PlayerTurnStartEvent -= cardSystemFlowActions.StartCardDrawTurn;
        playerTurn.PlayerTurnStartEvent -= uiSignalHubProvider.cardUISignalHub.PlayerTurnStarted;
        playerTurn.PlayerTurnStartEvent -= uiSignalHubProvider.gameplayUISignalHub.PlayerTurnStarted;

        ReleaseCharacter();
    }

    public void OnDestroy()
    {
        SpawnWaveEvent = null;
        ReleaseEvent();
    }

    public void Start()
    {
        gameStateMachine.ChangeState<GS_GameStarted>();
    }

    public bool IsState<T>() where T : GameState
    {
        return gameStateMachine.IsState<T>();
    }

    public T GetGameState<T>() where T : GameState
    {
        return gameStateMachine.GetState<T>();
    }

    public void ChangeGameState<T>() where T : GameState
    {
        gameStateMachine.ChangeState<T>();
    }

    public void PlayerTurnIsFinished()
    {
        ChangeGameState<GS_EnemyTurnState>();
    }

    public void ChangeGameStateToPlayerTurn()
    {
        ChangeGameState<GS_PlayerTurnState>();
    }

    public void WaveEnded()
    {

    }

    public void SpawnUnits()
    {
        SpawnWaveEvent?.Invoke(waveIdx);
        SpawnPlayerEvent?.Invoke();

        BindCharacter();
    }

    public void SpawnWave()
    {
        SpawnWaveEvent?.Invoke(waveIdx);
    }

    public void Release()
    {
        ReleaseEvent();
        ReleaseCharacter();
    }
}
