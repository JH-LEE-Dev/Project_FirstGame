using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BatteryCharge")]
public class EffectCommand_BatteryCharge : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private bool bUpgradedEffectOn = false;
    private bool bEffectOn = false;
    private BulletElementData data = new BulletElementData(BulletElementType.Electric, 1);
    private DebuffElementData debuff = new DebuffElementData(DebuffElementEffectType.ElectricShock, 2);
    private IComplexSystemActionCommandHandler handler = null;

    public override bool EffectConditionCheck()
    {
        if (handler == null)
            return false;

        var currentElement = handler.statusSystem.GetCurrentAppliedBulletElement();
        var inherenceCard = handler.cardSlotSystem.GetCurrentInherenceCard();

        int newCondition = 0;

        if (bUpgraded == false)
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
                return false;

            newCondition = 1;
        }
        else
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
            {
                newCondition = 2;
            }
            else
            {
                newCondition = 3;
            }
        }

        if (newCondition != condition)
        {
            condition = newCondition;
            CheckApplyCondition();
        }

        return true;
    }


    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        handler = _handler;

        bUpgradedEffectOn = false;
        bEffectOn = false;

        var currentElement = _handler.statusSystem.GetCurrentAppliedBulletElement();
        var inherenceCard = _handler.cardSlotSystem.GetCurrentInherenceCard();

        if (inherenceCard == null)
            return;
        if (EffectConditionCheck() == false)
            return;

        if (bUpgraded == false)
        {
            bEffectOn = true;

            _handler.statusSystem.ApplyBulletElementType(data);
        }
        else
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
            {
                bUpgradedEffectOn = true;

                _handler.statusSystem.ApplyDebuffElementType(debuff);
            }
            else
            {
                bEffectOn = true;

                _handler.statusSystem.ApplyBulletElementType(data);
            }
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        if (bUpgraded == false)
        {
            if (bEffectOn)
            {
                handler.statusSystem.UndoBulletElementApply(data);
            }
        }
        else
        {
            if (bUpgradedEffectOn)
            {
                handler.statusSystem.UndoDebuffElementApply(debuff);
            }
            else
            {
                if (bEffectOn)
                {
                    handler.statusSystem.UndoBulletElementApply(data);
                }
            }
        }
    }
}