using UnityEngine;

public interface ICardUISignalHub
{
    void PlayerTurnStarted(int waveIdx);

    void EnemyTurnStarted();
}
