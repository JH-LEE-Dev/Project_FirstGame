using UnityEngine;

public class EMoveComponent : MoveComponent
{

    protected override void Update()
    {
        if (moveDirection.x == 0)
            return;
    }

    protected override void Start()
    {
        base.Start();
    }
}
