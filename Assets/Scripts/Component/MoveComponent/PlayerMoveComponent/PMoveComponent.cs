using UnityEngine;

public class PMoveComponent : MoveComponent
{
    /// <summary>
    /// 시스템 속성 존.---------------------------------------
    /// </summary>
    
    //움직임 행동을 정의하는 객체. 원하는 MoveStrategy를 인스펙터에서 넣어주면 됨.
    [SerializeField] protected PlayerMoveStrategy moveStrategyAsset;
    protected PlayerMoveStrategy moveStrategy;

    //외부 의존성
    IOrbitPathProvider orbitPathProvider;




    /// <summary>
    /// 구현 속성 존.---------------------------------------
    /// </summary>








    /// <summary>
    /// 시스템 코드 존.---------------------------------------
    /// </summary>
    
    public void Initialize(UnitContext _ctx,IOrbitPathProvider _orbitPathProvider,IMoveSignalHandler _moveSignalHandler)
    {
        base.Initialize(_ctx, _moveSignalHandler);
        orbitPathProvider = _orbitPathProvider; 
    }

    protected override void Awake()
    {
        base.Awake();

        moveStrategy = Instantiate(moveStrategyAsset);
    }

    protected override void Start()
    {
        moveStrategy.Initialize(ctx.unit,orbitPathProvider);
    }

    protected override void Update()
    {
        base.Update();

        //if (moveDirection.x == 0)
        //    return;

        moveStrategy.Move(moveDirection);
    }



    /// <summary>
    /// 구현 코드 존.---------------------------------------
    /// </summary>

}
