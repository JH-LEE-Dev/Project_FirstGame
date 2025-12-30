using UnityEngine;
using System.Threading.Tasks;

public abstract class MoveStrategy : ScriptableObject
{
    protected Unit unit;
    public abstract void Initialize(Unit unit);

    public abstract void Move(Vector2 direction);

    public abstract void Move_Impulse(Vector2 direction, float power);
    public abstract Task AsyncMove(Vector2 direction);

    public abstract void Accelerate(Vector2 direction,float acceleration, float maxSpeed);
}