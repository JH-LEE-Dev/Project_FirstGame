using UnityEngine;

public class UIViewContext
{
    public InputManager inputManager { get; private set; }
    public ICardSystemProvider cardSystemProvider { get; private set; }

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;
    }

    public void Initialize_Gameplay(ICardSystemProvider _cardSystemProvider)
    {
        cardSystemProvider = _cardSystemProvider;
    }

    public void ReleaseDependency_GameplayScene()
    {
        cardSystemProvider = null;
    }
}
