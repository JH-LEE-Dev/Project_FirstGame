using UnityEngine;

//Character 고유의 방식으로 각 컴포넌트와 통신해야 할 경우 이 구체 클래스에 작성.
public class PVisualComponentCoordinator : VisualComponentCoordinator
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    private PCombatComponent combatComponent;
    private PMoveComponent moveComponent;
    private Character_Visual characterVisualComponent;
    private CutsceneComponent cutsceneComponent;




    /// <summary>
    /// 구현 속성 존. -----------------------------------------
    /// </summary>















    /// <summary>
    /// 시스템 코드 존. -----------------------------------------
    /// </summary>

    public void Initialize(Character_Visual _characterVisual, PCombatComponent _combatComponent, PMoveComponent _moveComponent, CutsceneComponent _cutsceneComponent)
    {
        characterVisualComponent = _characterVisual;
        combatComponent = _combatComponent;
        moveComponent = _moveComponent;
        cutsceneComponent = _cutsceneComponent;
    }

    //여기에서 이벤트를 바인딩하면 됨.
    private void BindEvent()
    {

    }







    /// <summary>
    /// 구현 코드 존. -----------------------------------------
    /// </summary>
    /// 
    public override void NotifyCombatActionSignal(CombatActionSignal signal)
    {

    }

    public override void NotifyMoveSignalAction(MoveActionSignal signal)
    {
        switch (signal)
        {
            case MoveActionSignal.RightMoving:
                characterVisualComponent.Flip(Dir.Right);
                break;
            case MoveActionSignal.LeftMoving:
                characterVisualComponent.Flip(Dir.Left);
                break;
        }
    }

    public override void NotifyCutsceneSignalAction(CutsceneSignal signal)
    {
        switch (signal)
        {
            // 카드 사용 시작 연출 시작 (드로우 시점)
            case CutsceneSignal.TurnStart_Start:
                characterVisualComponent.Flip(Dir.Right);
                // 기존 위치로 빨려가면안됨.
                moveComponent.SetbIgnorePath(true);
                Debug.LogWarning("TurnStart_Start");
                break;

            // 카드 사용 시작 연출 종료
            case CutsceneSignal.TurnStart_End:
                moveComponent.SetbIgnorePath(true);
                Debug.LogWarning("TurnStart_End");

                break;

            // 턴 넘기기 버튼 클릭
            case CutsceneSignal.TurnEnd_Start:
                // 여전히 빨려 들어가면안됨.
                moveComponent.SetbIgnorePath(true);
                Debug.LogWarning("TurnEnd_Start");

                break;

            // 턴 넘기기 버튼 클릭 연출 종료
            case CutsceneSignal.TurnEnd_End:
                // 턴 종료 연출이 끝났으므로, 빨려가게 방치하고 위치를 리셋해준다.
                moveComponent.SetbIgnorePath(false);
                moveComponent.ResetCharacterPosition();
                Debug.LogWarning("TurnEnd_End");
                break;
        }
    }
}
