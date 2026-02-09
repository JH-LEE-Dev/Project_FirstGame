using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Natural Cycle Execute")]
public class EffectCommand_NaturalCycle_Execute : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    protected override void Execute(ICardLogicSystemActionCommandHandler logicSystemActionCommandHandler)
    {
        if(bUpgraded == false)
        {
            logicSystemActionCommandHandler.DrawAgain(2);
        }
        else
        {
            logicSystemActionCommandHandler.DrawAgain(3);
        }
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler logicSystemActionCommandHandler)
    {

    }
}