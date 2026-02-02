using UnityEngine;

public class EMoveComponent : MoveComponent
{
    /// <summary>
    /// 시스템 속성 존.---------------------------------------
    /// </summary>
    
    //움직임 행동을 정의하는 객체. 원하는 MoveStrategy를 인스펙터에서 넣어주면 됨.
    [SerializeField] protected EnemyMoveStrategy moveStrategyAsset;
    protected EnemyMoveStrategy moveStrategy;




    /// <summary>
    /// 구현 속성 존.---------------------------------------
    /// </summary>
    [SerializeField] float acceleration = 8f;
    [SerializeField] float maxSpeed = 20f;




    /// <summary>
    /// 시스템 코드 존.---------------------------------------
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


    /// <summary>
    /// 구현 코드 존.---------------------------------------
    /// </summary>

    protected override void Update()
    {
        if (moveDirection.x == 0)
            return;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (bAccelerate && ctx.unit.IsUnitDead() == false)
            Accelerate(); // 지구로 돌진할 때 호출되는 함수임.
    }

    private void Accelerate()
    {
        moveStrategy.Accelerate(moveDirection, acceleration, maxSpeed);
    }

    public void SetImpulsePower(float power)
    {
        impulsePower = power;
    }

    public void SetAccelerate(bool boolean)
    {
        bAccelerate = boolean;
    }

    public virtual void ApplyImpulse()
    {
        moveStrategy.Move_Impulse(moveDirection, impulsePower);
    }

    public void ApplyKnockBack(Vector2 dir,float power)
    {
        moveStrategy.KnockBack(dir,power);
    }
}
