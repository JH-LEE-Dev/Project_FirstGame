using UnityEngine;

public interface IGameFlowProvider
{
    public bool IsState<T>() where T : IState;

    public T GetGameState<T>() where T : IState;
}
