using UnityEngine;

[CreateAssetMenu(menuName = "Command/StatusSystemAction/SetPlayerAttackState")]
public class ActionCommand_SetCharacterCanAttackState : CardSystemActionCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        _handler.statusSystem.SetCharacterCanAttackState(false);
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {

    }
}
