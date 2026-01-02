using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public event Action<int> waveIdxDeclareEvent;

    private WaveManager waveManager;
    private GameStateMachine gameStateMachine;
    private InputManager inputManager;
    private IDeckProvider deckProvider;

    private int waveIdx = 0;

    public void Initialize(WaveManager _waveManager, InputManager _inputManager, IDeckProvider _deckProvider)
    {
        waveManager = _waveManager;
        inputManager = _inputManager;
        deckProvider = _deckProvider;
        gameStateMachine = new GameStateMachine();

        GS_PlayerTurnState PlayerTurn = new GS_PlayerTurnState();
        waveIdxDeclareEvent += PlayerTurn.SetWaveIdx;
        gameStateMachine.AddState(PlayerTurn);
    }

    public void Start()
    {
        GS_PlayerTurnState playerTurnState = gameStateMachine.GetState<GS_PlayerTurnState>();

        if (playerTurnState != null)
        {
            playerTurnState.PlayerTurnStartEvent += waveManager.SpawnWave;
        }

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

    public void OnDisable()
    {
        
    }
}
