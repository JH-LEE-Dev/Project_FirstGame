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
    IOrbitPathProvider orbirPathProvider;




    /// <summary>
    /// 구현 속성 존.---------------------------------------
    /// </summary>

    // Path에 빨려가는것을 막기위한 장치
    private bool bIgnorePath = false;






    /// <summary>
    /// 시스템 코드 존.---------------------------------------
    /// </summary>
    
    public void Initialize(UnitContext _ctx, IOrbitPathProvider _orbitPathProvider, IMoveSignalHandler _moveSignalHandler)
    {
        base.Initialize(_ctx, _moveSignalHandler);
        orbirPathProvider = _orbitPathProvider;
    }

    protected override void Awake()
    {
        base.Awake();

        moveStrategy = Instantiate(moveStrategyAsset);
    }

    protected override void Start()
    {
        moveStrategy.Initialize(ctx.unit, orbirPathProvider, moveSignalHandler);
    }

    protected override void Update()
    {
        base.Update();

        // 컷씬 연출 중일땐 레일로 빨려가는것을 막는다.
        if (bIgnorePath == true) return;

        moveStrategy.Move(moveDirection);
    }

    /// <summary>
    /// 구현 코드 존.---------------------------------------
    /// </summary>

    public void SetbIgnorePath(bool value)
    {
        bIgnorePath = value;
    }

    public void ResetCharacterPosition()
    {
        moveStrategy.ResetCharacterPosition();
    }

    public Vector3 GetCharacterResetPosition()
    {
        return moveStrategy.GetCharacterResetPosition();
    }
}
