using UnityEngine;

//Character 고유의 방식으로 각 컴포넌트와 통신해야 할 경우 이 구체 클래스에 작성.
public class PVisualComponentCoordinator : VisualComponentCoordinator
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    private PCombatComponent combatComponent;
    private PMoveComponent moveComponent;
    //private CharacterVisualComponent;





    /// <summary>
    /// 구현 속성 존. -----------------------------------------
    /// </summary>















    /// <summary>
    /// 시스템 코드 존. -----------------------------------------
    /// </summary>

    public void Initialize(PCombatComponent _combatComponent, PMoveComponent _moveComponent)
    {
        combatComponent = _combatComponent;
        moveComponent = _moveComponent;
    }

    //여기에서 이벤트를 바인딩하면 됨.
    private void BindEvent()
    {

    }







    /// <summary>
    /// 구현 코드 존. -----------------------------------------
    /// </summary>
}
