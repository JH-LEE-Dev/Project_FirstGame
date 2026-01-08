using UnityEngine;
using System.Threading.Tasks;

public class MoveComponent : EntityComponent
{
    /// <summary>
    /// 시스템 속성 존. -------------------------------------------
    /// </summary>
    
    //움직임 행동을 정의하는 객체. 원하는 MoveStrategy를 인스펙터에서 넣어주면 됨.
    [SerializeField] protected MoveStrategy moveStrategyAsset; 
    protected MoveStrategy moveStrategy;

    /// <summary>
    /// 구현 속성 존. ---------------------------------------------
    /// </summary>
    protected Vector2 moveDirection;
    protected float impulsePower;
    protected bool bAccelerate = false;





    /// <summary>
    /// 시스템 코드 존. -------------------------------------------
    /// </summary>

    protected override void Awake()
    {
        base.Awake();

        moveStrategy = Instantiate(moveStrategyAsset);
    }

    protected override void Start()
    {
        moveStrategy.Initialize(ctx.unit);
    }

    //미구현 신경 끄쇼.
    public virtual async void AsyncMove()
    {
        await moveStrategy.AsyncMove(moveDirection);
    }

    //미구현 신경 끄쇼.
    public virtual void ApplyImpulse()
    {
        moveStrategy.Move_Impulse(moveDirection, impulsePower);
    }

    //Enemy 전용. 신경 끄쇼
    public virtual void SetImpulsePower(float power)
    {

    }

    //Enemy 전용. 신경 끄쇼
    public virtual void SetAccelerate(bool boolean)
    {

    }


    /// <summary>
    /// 구현 코드 존. --------------------------------------------
    /// </summary>

    protected override void Update()
    {
        if (moveDirection.x == 0)
            return;

        moveStrategy.Move(moveDirection);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();


    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }
}
