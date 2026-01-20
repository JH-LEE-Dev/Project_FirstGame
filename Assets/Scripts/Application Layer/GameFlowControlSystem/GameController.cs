using GameControlSignals;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using WaveSystemSignals;

public class GameController : MonoBehaviour
{
    //외부 의존성.
    private SignalHub signalHub;

    //내부 의존성
    private GameStateMachine gameStateMachine;

    public void Initialize(SignalHub _signalHub)
    {
        signalHub = _signalHub; 
        gameStateMachine = new GameStateMachine();

        SetupGameState();

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        signalHub.Subscribe<WaveMoveEndEvent>(ChangeGameStateToPlayerTurn);
    }

    private void UnSubscribeEvents()
    {
        signalHub.UnSubscribe<WaveMoveEndEvent>(ChangeGameStateToPlayerTurn);
    }

    public void SetupGameController()
    {
        BindEvent();
    }

    private void SetupGameState()
    {
        GS_PlayerTurn playerTurn = new GS_PlayerTurn();
        playerTurn.Initialize(signalHub,gameStateMachine);
        gameStateMachine.AddState(playerTurn);

        GS_EnemyTurn enemyTurn = new GS_EnemyTurn();
        enemyTurn.Initialize(signalHub,gameStateMachine);
        gameStateMachine.AddState(enemyTurn);

        GS_GameStarted gameStarted = new GS_GameStarted();
        gameStarted.Initialize(signalHub,gameStateMachine);
        gameStateMachine.AddState(gameStarted);

        GS_WaveStarted waveStarted = new GS_WaveStarted();
        waveStarted.Initialize(signalHub, gameStateMachine);
        gameStateMachine.AddState(waveStarted);

        GS_WaveEnded waveEnded = new GS_WaveEnded();
        waveEnded.Initialize(signalHub, gameStateMachine);
        gameStateMachine.AddState(waveEnded);

        GS_ShopTime shopTime = new GS_ShopTime();
        shopTime.Initialize(signalHub, gameStateMachine);
        gameStateMachine.AddState(shopTime);
    }

    public void GameStart()
    {
        gameStateMachine.ChangeState<GS_GameStarted>();
    }

    private void BindEvent()
    {
    }

    private void ReleaseEvent()
    {
    }

    public void OnDestroy()
    {
        ReleaseEvent();
    }

    private void Start()
    {

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

    public void ChangeGameStateToPlayerTurn(WaveMoveEndEvent waveMoveEndEvent)
    {
        ChangeGameState<GS_PlayerTurn>();
    }

    public void Release()
    {
        UnSubscribeEvents();
        gameStateMachine.ReleaseAllState();
        ReleaseEvent();
    }

    private void ReleaseAllState()
    {

    }
}
