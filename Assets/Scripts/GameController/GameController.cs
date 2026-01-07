using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private float EnemyMoveDelay = 2f;

    public event Action<int> waveIdxDeclareEvent;

    private WaveManager waveManager;
    private GameStateMachine gameStateMachine;
    private InputManager inputManager;
    private CardManager cardManager;

    private int waveIdx = 0;

    public void Initialize(WaveManager _waveManager, InputManager _inputManager, CardManager _cardManager)
    {
        waveManager = _waveManager;
        inputManager = _inputManager;
        cardManager = _cardManager;
        gameStateMachine = new GameStateMachine();

        GS_PlayerTurnState playerTurn = new GS_PlayerTurnState();
        gameStateMachine.AddState(playerTurn);

        GS_EnemyTurnState enemyTurn = new GS_EnemyTurnState();
        gameStateMachine.AddState(enemyTurn);

        BindEvent(enemyTurn, playerTurn);
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

    public void OnDestroy()
    {
        ReleaseEvent();
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

    public void BindEvent(GS_EnemyTurnState enemyTurn,GS_PlayerTurnState playerTurn)
    {
        enemyTurn.EnemyTurnStartEvent -= waveManager.StartEnemyMoveTurn;
        enemyTurn.EnemyTurnStartEvent += waveManager.StartEnemyMoveTurn;

        waveManager.WaveMoveEndEvent -= ChangeGameStateToPlayerTurn;
        waveManager.WaveMoveEndEvent += ChangeGameStateToPlayerTurn;

        playerTurn.PlayerTurnStartEvent -= cardManager.StartDraw;
        playerTurn.PlayerTurnStartEvent += cardManager.StartDraw;
    }

    public void ReleaseEvent()
    {
        GS_PlayerTurnState playerTurn = gameStateMachine.GetState<GS_PlayerTurnState>();
        GS_EnemyTurnState enemyTurn = gameStateMachine.GetState<GS_EnemyTurnState>();

        enemyTurn.EnemyTurnStartEvent -= waveManager.StartEnemyMoveTurn;

        waveManager.WaveMoveEndEvent -= ChangeGameStateToPlayerTurn;

        playerTurn.PlayerTurnStartEvent -= cardManager.StartDraw;
    }
}
