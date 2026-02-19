using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BatteryCharge")]
public class EffectCommand_BatteryCharge : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private bool bUpgradedEffectOn = false;
    private bool bEffectOn = false;
    private BulletElementData data = new BulletElementData(BulletElementType.Electric, 1);
    private DebuffElementData debuff = new DebuffElementData(DebuffElementEffectType.ElectricShock, 5);

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        bUpgradedEffectOn = false;
        bEffectOn = false;

        var currentElement = complexSystemActionCommandHandler.GetCurrentAppliedBulletElement();
        var inherenceCard = complexSystemActionCommandHandler.GetCurrentInherenceCard();

        if (inherenceCard == null)
            return;

        if (bUpgraded == false)
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
                return;

            bEffectOn = true;

            complexSystemActionCommandHandler.ApplyBulletElementType(data);
        }
        else
        {
            if (currentElement.ContainsKey(BulletElementType.Electric))
            {
                bUpgradedEffectOn = true;

                complexSystemActionCommandHandler.ApplyDebuffElementType(debuff);
            }
            else
            {
                bEffectOn = true;

                complexSystemActionCommandHandler.ApplyBulletElementType(data);
            }
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (bUpgraded == false)
        {
            if (bEffectOn)
            {
                complexSystemActionCommandHandler.UndoBulletElementApply(data);
            }
        }
        else
        {
            if (bUpgradedEffectOn)
            {
                complexSystemActionCommandHandler.UndoDebuffElementApply(debuff);
            }
            else
            {
                if (bEffectOn)
                {
                    complexSystemActionCommandHandler.UndoBulletElementApply(data);
                }
            }
        }
    }
}