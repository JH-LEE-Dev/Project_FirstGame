using UnityEngine;

//Enemy 고유의 방식으로 각 컴포넌트와 통신해야 할 경우 이 구체 클래스에 작성.
public class EVisualComponentCoordinator : VisualComponentCoordinator
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>
    
    private ECombatComponent combatComponent;
    private EMoveComponent moveComponent;
    //private EnemyVisualComponent visualComponent;






    /// <summary>
    /// 구현 속성 존. -----------------------------------------
    /// </summary>













    /// <summary>
    /// 시스템 코드 존. -----------------------------------------
    /// </summary>

    public void Initialize(ECombatComponent _combatComponent,EMoveComponent _moveComponent)
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

    public override void NotifyCombatActionSignal(CombatActionSignal signal)
    {
        //예시
        //visualComponent,TriggerCombatActionEffect(signal);
    }

    public override void NotifyMoveSignalAction(MoveActionSignal signal)
    {
        //visualComponent,TriggerMoveActionEffect(signal);
    }
}
