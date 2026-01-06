using UnityEngine;

public class UIViewContext
{
    public ICardSystemProvider cardSystemProvider;
    public InputManager inputManager;

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;
    }

    public void Initialize_Gameplay(ICardSystemProvider _cardSystemProvider)
    {
        cardSystemProvider = _cardSystemProvider;
    }

    public void ResetVariable()
    {
        cardSystemProvider = null;
    }
}
