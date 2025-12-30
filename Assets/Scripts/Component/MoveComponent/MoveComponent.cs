using UnityEngine;
using System.Threading.Tasks;

public class MoveComponent : EntityComponent
{
    [SerializeField] protected MoveStrategy moveStrategyAsset;
    protected MoveStrategy moveStrategy;

    protected Vector2 moveDirection;
    protected float impulsePower;
    protected bool bAccelerate = false;

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

    protected override void Awake()
    {
        base.Awake();

        moveStrategy = Instantiate(moveStrategyAsset);
    }

    protected override void Start()
    {
        moveStrategy.Initialize(ctx.unit);
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void SetImpulsePower(float power)
    {
        impulsePower = power;
    }

    public void SetAccelerate(bool boolean)
    {
        bAccelerate = boolean;
    }

    public virtual async void AsyncMove()
    {
        await moveStrategy.AsyncMove(moveDirection);
    }

    public virtual void ApplyImpulse()
    {
        moveStrategy.Move_Impulse(moveDirection,impulsePower);
    }
}
