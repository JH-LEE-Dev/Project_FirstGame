using UnityEngine;

public class UIViewContext
{
    public IDeckProvider deckProvider;
    public InputManager inputManager;

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;
    }

    public void Initialize_Gameplay(IDeckProvider _deckProvider)
    {
        deckProvider = _deckProvider;
    }

    public void ResetVariable()
    {
        deckProvider = null;
    }
}
