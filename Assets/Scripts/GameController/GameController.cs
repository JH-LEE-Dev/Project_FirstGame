using System;
using UnityEngine;

public class GameController : MonoBehaviour, IGameFlowController
{
    public event Action<int> SpawnWaveEvent;

    //외부 의존성.
    private InputManager inputManager;
    private ICardSystemFlowActions cardSystemFlowActions;
    private IWaveSystemActions waveSystemActions;

    //내부 의존성
    private GameStateMachine gameStateMachine;


    private int waveIdx = 0;

    public void Initialize(IWaveSystemActions _waveSystemActions, InputManager _inputManager,
        ICardSystemFlowActions _cardSystemFlowActions)
    {
        waveSystemActions = _waveSystemActions;
        inputManager = _inputManager;
        cardSystemFlowActions = _cardSystemFlowActions;
        gameStateMachine = new GameStateMachine();

        SetGameState();
    }

    private void SetGameState()
    {
        GS_PlayerTurnState playerTurn = new GS_PlayerTurnState();
        gameStateMachine.AddState(playerTurn);

        GS_EnemyTurnState enemyTurn = new GS_EnemyTurnState();
        gameStateMachine.AddState(enemyTurn);

        BindEvent(enemyTurn, playerTurn);
    }

    public void BindEvent(GS_EnemyTurnState enemyTurn, GS_PlayerTurnState playerTurn)
    {
        enemyTurn.EnemyTurnStartEvent -= waveSystemActions.StartEnemyMoveTurn;
        enemyTurn.EnemyTurnStartEvent += waveSystemActions.StartEnemyMoveTurn;

        playerTurn.PlayerTurnStartEvent -= cardSystemFlowActions.StartCardDrawTurn;
        playerTurn.PlayerTurnStartEvent += cardSystemFlowActions.StartCardDrawTurn;
    }

    public void ReleaseEvent()
    {
        GS_PlayerTurnState playerTurn = gameStateMachine.GetState<GS_PlayerTurnState>();
        GS_EnemyTurnState enemyTurn = gameStateMachine.GetState<GS_EnemyTurnState>();

        enemyTurn.EnemyTurnStartEvent -= waveSystemActions.StartEnemyMoveTurn;
        playerTurn.PlayerTurnStartEvent -= cardSystemFlowActions.StartCardDrawTurn;
    }

    public void OnDestroy()
    {
        SpawnWaveEvent = null;
        ReleaseEvent();
    }

    public void Start()
    {
        SpawnWaveEvent?.Invoke(waveIdx);
        gameStateMachine.ChangeState<GS_PlayerTurnState>();
    }

    public bool IsState<T>() where T : IState
    {
        return gameStateMachine.IsState<T>();
    }

    public T GetGameState<T>() where T : IState
    {
        return gameStateMachine.GetState<T>();
    }

    public void ChangeGameState<T>() where T : IState
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
}
