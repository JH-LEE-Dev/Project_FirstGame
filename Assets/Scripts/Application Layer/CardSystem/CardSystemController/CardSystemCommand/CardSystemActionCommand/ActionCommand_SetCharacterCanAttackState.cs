using UnityEngine;

[CreateAssetMenu(menuName = "Command/StatusSystemAction/SetPlayerAttackState")]
public class ActionCommand_SetCharacterCanAttackState : CardSystemActionCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        complexSystemActionCommand.SetCharacterCanAttackState(false);
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {

    }
}
