using UnityEngine;

public interface IGameFlowController : IGameFlowProvider
{
    void PlayerTurnIsFinished();
}
