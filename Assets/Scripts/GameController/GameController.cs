using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private WaveManager waveManager;
    private GameStateMachine gameStateMachine;
    private InputManager inputManager;
    private DeckManager deckManager;

    public void Initialize(WaveManager _waveManager, InputManager _inputManager, DeckManager _deckManager)
    {
        waveManager = _waveManager;
        inputManager = _inputManager;
        deckManager = _deckManager;
        gameStateMachine = new GameStateMachine();

        GS_PlayerTurnState PlayerTurn = new GS_PlayerTurnState();

        gameStateMachine.AddState(PlayerTurn);

    }

    public void Start()
    {
        waveManager.SpawnWave(0);
    }

    public bool IsState<T>() where T : IState
    {
        return gameStateMachine.IsState<T>();
    }

    public void OnDisable()
    {
        
    }
}
