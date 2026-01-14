using UnityEngine;

public class GameplayUICoordinator : IGameplayUISignalHub
{
    private UIView_HUD hudUISystem;
    private UIView_Unit unitUISystem;
    private UIView_Gameplay gameplayUISystem;

    public void Initialize(UIView_HUD _hudUISystem, UIView_Unit _unitUISystem,UIView_Gameplay _gameplayUISystem)
    {
        hudUISystem = _hudUISystem;
        unitUISystem = _unitUISystem;
        gameplayUISystem = _gameplayUISystem;
    }

    public void PlayerTurnStarted(int waveIdx)
    {

    }

    public void EnemyTurnStarted()
    {

    }

    public void CardUseTimeStarted()
    {
        hudUISystem.CardUseTimeStarted();
    }

    public void PlayerSpawned(IPlayerData _playerData)
    {
        hudUISystem.Initialize(_playerData);
    }

    public void CardUsingFinished()
    {

    }
}
