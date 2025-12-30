using UnityEngine;

public interface IState
{
    void Imitialize();
    void Enter();
    void Exit();
    void Update();
}