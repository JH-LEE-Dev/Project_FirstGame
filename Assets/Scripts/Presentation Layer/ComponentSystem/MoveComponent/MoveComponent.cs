using UnityEngine;
using System.Threading.Tasks;

public class MoveComponent : EntityComponent
{
    /// <summary>
    /// 시스템 속성 존. -------------------------------------------
    /// </summary>

    protected IMoveSignalHandler moveSignalHandler;


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

    public void Initialize(UnitContext _ctx,IMoveSignalHandler _moveSignalHandler)
    {
        base.Initialize(_ctx);

        moveSignalHandler = _moveSignalHandler;
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
