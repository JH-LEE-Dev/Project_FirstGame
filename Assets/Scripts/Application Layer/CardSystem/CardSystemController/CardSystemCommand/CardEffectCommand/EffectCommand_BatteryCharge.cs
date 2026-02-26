using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BatteryCharge")]
public class EffectCommand_BatteryCharge : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private bool bUpgradedEffectOn = false;
    private bool bEffectOn = false;
    private BulletElementData data = new BulletElementData(BulletElementType.Electric, 1);
    private DebuffElementData debuff = new DebuffElementData(DebuffElementEffectType.ElectricShock, 2);

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        bUpgradedEffectOn = false;
        bEffectOn = false;

        var currentElement = handler.statusSystem.GetCurrentAppliedBulletElement();
        var inherenceCard = handler.cardSlotSystem.GetCurrentInherenceCard();

        if (inherenceCard == null)
            return;

        if (bUpgraded == false)
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
                return;

            bEffectOn = true;

            handler.statusSystem.ApplyBulletElementType(data);
        }
        else
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
            {
                bUpgradedEffectOn = true;

                handler.statusSystem.ApplyDebuffElementType(debuff);
            }
            else
            {
                bEffectOn = true;

                handler.statusSystem.ApplyBulletElementType(data);
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