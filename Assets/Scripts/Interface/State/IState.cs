using UnityEngine;

public interface IState
{
    void SetWaveIdx(int idx);
    void Initialize();
    void Enter();
    void Exit();
    void Update();

    void Release();
}