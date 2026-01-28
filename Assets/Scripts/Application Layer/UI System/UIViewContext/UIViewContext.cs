using UnityEngine;

public class UIViewContext
{
    public InputManager inputManager { get; private set; }
    public ICardLocalizationSystem cardLocalizationSystem { get; private set; }

    public void Initialize(InputManager _inputManager, ICardLocalizationSystem _cardLocalizationSystem)
    {
        inputManager = _inputManager;
        cardLocalizationSystem = _cardLocalizationSystem;
    }

    public void Initialize_Gameplay()
    {

    }

    public void ReleaseDependency()
    {

    }
}
