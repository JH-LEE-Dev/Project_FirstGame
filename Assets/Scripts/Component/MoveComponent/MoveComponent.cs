using UnityEngine;
using System.Threading.Tasks;

public class MoveComponent : EntityComponent
{
    /// <summary>
    /// 시스템 속성 존. -------------------------------------------
    /// </summary>
    


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
    }

    protected override void Start()
    {

    }


    /// <summary>
    /// 구현 코드 존. --------------------------------------------
    /// </summary>

    protected override void Update()
    {

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
