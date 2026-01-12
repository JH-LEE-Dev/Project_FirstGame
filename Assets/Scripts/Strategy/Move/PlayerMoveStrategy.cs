using UnityEngine;
using System.Threading.Tasks;

public abstract class PlayerMoveStrategy : ScriptableObject
{
    protected Unit unit;
    protected IOrbitPathProvider orbitPathProvider;

    //Initialize함수.
    public abstract void Initialize(Unit unit, IOrbitPathProvider orbitPathProvider);

    //움직임 함수. 실질적으로 매 프레임마다 호출됨.
    public abstract void Move(Vector2 direction);

}