using UnityEngine;

public interface IState
{
    void Initialize();
    void Enter();
    void Exit();
    void Update();
}