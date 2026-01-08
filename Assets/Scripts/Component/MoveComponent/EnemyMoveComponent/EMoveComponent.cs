using UnityEngine;

public class EMoveComponent : MoveComponent
{
    [SerializeField] float acceleration = 8f;
    [SerializeField] float maxSpeed = 20f;

    protected override void Update()
    {
        if (moveDirection.x == 0)
            return;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (bAccelerate && bDead == false)
            Accelerate();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Accelerate()
    {
        moveStrategy.Accelerate(moveDirection, acceleration, maxSpeed);
    }
}
