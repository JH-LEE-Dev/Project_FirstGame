using UnityEngine;

public class UIModuleCoordinator 
{
    private CardUICoordinator cardUICoordinator;
    private GameplayUICoordinator gameplayUICoordinator;

    public void Initialize(CardUICoordinator _cardUICoordinator, GameplayUICoordinator _gameplayUICoordinator)
    {
        cardUICoordinator = _cardUICoordinator; 
        gameplayUICoordinator = _gameplayUICoordinator;

        BindEvents();
    }

    public void BindEvents()
    {

    }

    public void ReleaseEvents()
    {

    }

    public void Release()
    {
        ReleaseEvents();
    }
}
