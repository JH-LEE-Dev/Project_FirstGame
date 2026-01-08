using UnityEngine;
using System.Threading.Tasks;

public abstract class MoveStrategy : ScriptableObject
{
    //유닛에 대한 의존성.
    protected Unit unit;

    //Initialize함수.
    public abstract void Initialize(Unit unit);

    //움직임 함수. 실질적으로 매 프레임마다 호출됨.
    public abstract void Move(Vector2 direction);

    //이건 RigidBody를 활용한 Impulse 방식의 움직임 함수. 한 번만 트리거됨.
    //거의 Enemy 전용.
    public abstract void Move_Impulse(Vector2 direction, float power);

    //미구현임. 신경쓰지 마셈.
    public abstract Task AsyncMove(Vector2 direction);

    //가속하는 함수. 거의 Enemy전용.
    public abstract void Accelerate(Vector2 direction,float acceleration, float maxSpeed);
}