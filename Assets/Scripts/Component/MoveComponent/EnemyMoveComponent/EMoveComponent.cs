using UnityEngine;

public class EMoveComponent : MoveComponent
{
    /// <summary>
    /// 시스템 속성 존.---------------------------------------
    /// </summary>




    /// <summary>
    /// 구현 속성 존.---------------------------------------
    /// </summary>
    [SerializeField] float acceleration = 8f;
    [SerializeField] float maxSpeed = 20f;


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

        if (bAccelerate && bDead == false)
            Accelerate(); // 지구로 돌진할 때 호출되는 함수임.
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Accelerate()
    {
        moveStrategy.Accelerate(moveDirection, acceleration, maxSpeed);
    }

    public override void SetImpulsePower(float power)
    {
        impulsePower = power;
    }

    public override void SetAccelerate(bool boolean)
    {
        bAccelerate = boolean;
    }
}
