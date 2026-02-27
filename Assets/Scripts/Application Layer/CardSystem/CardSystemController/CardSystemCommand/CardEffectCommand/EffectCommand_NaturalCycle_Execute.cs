using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Natural Cycle Execute")]
public class EffectCommand_NaturalCycle_Execute : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    private bool bExecuted = false;

    public override bool EffectConditionCheck()
    {
        int newCondition = 0;

        if (newCondition != condition)
        {
            CheckApplyCondition();
            condition = newCondition;
        }
        return true;
    }

    public override void ResetCommandData()
    {
        base.ResetCommandData();

        bExecuted = false;    
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler logicSystemActionCommandHandler)
    {
        if (bExecuted == true)
            return;

        if(bUpgraded == false)
        {
            logicSystemActionCommandHandler.DrawAgain(2);
        }
        else
        {
            logicSystemActionCommandHandler.DrawAgain(3);
        }

        bExecuted = true;
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler logicSystemActionCommandHandler)
    {

    }
}