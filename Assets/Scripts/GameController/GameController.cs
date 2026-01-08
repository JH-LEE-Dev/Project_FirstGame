using System;
using UnityEngine;

public class GameController : MonoBehaviour, IGameFlowController
{
    //외부 의존성.
    private InputManager inputManager;
    private ICardSystemActions cardSystemActions;

    //내부 의존성
    private GameStateMachine gameStateMachine;
    //내부 의존성이지만 최상위 모듈에서 주입해줌.
    private WaveManager waveManager;

    private int waveIdx = 0;

    public void Initialize(WaveManager _waveManager, InputManager _inputManager,
        ICardSystemActions _cardSystemActions)
    {
        waveManager = _waveManager;
        inputManager = _inputManager;
        cardSystemActions = _cardSystemActions;
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
        enemyTurn.EnemyTurnStartEvent -= waveManager.StartEnemyMoveTurn;
        enemyTurn.EnemyTurnStartEvent += waveManager.StartEnemyMoveTurn;

        waveManager.WaveMoveEndEvent -= ChangeGameStateToPlayerTurn;
        waveManager.WaveMoveEndEvent += ChangeGameStateToPlayerTurn;

        playerTurn.PlayerTurnStartEvent -= cardSystemActions.StartDraw;
        playerTurn.PlayerTurnStartEvent += cardSystemActions.StartDraw;
    }

    public void ReleaseEvent()
    {
        GS_PlayerTurnState playerTurn = gameStateMachine.GetState<GS_PlayerTurnState>();
        GS_EnemyTurnState enemyTurn = gameStateMachine.GetState<GS_EnemyTurnState>();

        enemyTurn.EnemyTurnStartEvent -= waveManager.StartEnemyMoveTurn;

        waveManager.WaveMoveEndEvent -= ChangeGameStateToPlayerTurn;

        playerTurn.PlayerTurnStartEvent -= cardSystemActions.StartDraw;
    }
    public void OnDestroy()
    {
        ReleaseEvent();
    }

    public void Start()
    {
        waveManager.SpawnWave(waveIdx);
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
}
