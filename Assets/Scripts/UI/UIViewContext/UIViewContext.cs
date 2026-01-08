using UnityEngine;

public class UIViewContext
{
    public InputManager inputManager { get; private set; }
    public ICardSystemStatus cardSystemStatus { get; private set; }

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;
    }

    public void Initialize_Gameplay(ICardSystemStatus _cardSystemStatus)
    {
        cardSystemStatus = _cardSystemStatus;
    }

    public void ReleaseDependency_GameplayScene()
    {
        cardSystemStatus = null;
    }
}
