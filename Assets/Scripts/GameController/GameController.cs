using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private WaveManager waveManager;
    private GameStateMachine gameStateMachine;
    private InputManager inputManager;
    private IDeckProvider deckProvider;

    public void Initialize(WaveManager _waveManager, InputManager _inputManager, IDeckProvider _deckProvider)
    {
        waveManager = _waveManager;
        inputManager = _inputManager;
        deckProvider = _deckProvider;
        gameStateMachine = new GameStateMachine();

        GS_PlayerTurnState PlayerTurn = new GS_PlayerTurnState();

        gameStateMachine.AddState(PlayerTurn);

    }

    public void Start()
    {
        waveManager.SpawnWave(0);

        gameStateMachine.ChangeState<GS_PlayerTurnState>();
    }

    public bool IsState<T>() where T : IState
    {
        return gameStateMachine.IsState<T>();
    }

    public void OnDisable()
    {
        
    }
}
