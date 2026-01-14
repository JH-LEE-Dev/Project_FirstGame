using UnityEngine;

public interface IGameplayUISignalHub 
{
    void PlayerTurnStarted(int waveIdx);

    void EnemyTurnStarted();
}
