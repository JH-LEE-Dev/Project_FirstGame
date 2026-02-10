using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BatteryCharge")]
public class EffectCommand_BatteryCharge : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private bool bUpgradedEffectOn = false;
    private bool bEffectOn = false;
    private BulletElementData data = new BulletElementData(BulletElementType.Electric, 1);
    private DebuffElementData debuff = new DebuffElementData(DebuffElementEffectType.ElectricShock, 2);

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        bUpgradedEffectOn = false;
        bEffectOn = false;

        var card = complexSystemActionCommandHandler.GetCurrentInherenceCard();

        if (bUpgraded == false)
        {
            if (card.elementTypes.ContainsKey(BulletElementType.Electric))
                return;

            bEffectOn = true;
            card.elementTypes[BulletElementType.Electric] = data;
        }
        else
        {
            if (card.elementTypes.ContainsKey(BulletElementType.Electric))
            {
                bUpgradedEffectOn = true;

                if (card.debuffTypes.ContainsKey(DebuffElementEffectType.ElectricShock))
                {
                    var data = card.debuffTypes[DebuffElementEffectType.ElectricShock];
                    data.turnCnt += 2;
                    card.debuffTypes[DebuffElementEffectType.ElectricShock] = data;
                }
                else
                {
                    card.debuffTypes[DebuffElementEffectType.ElectricShock] = debuff;
                }
            }
            else
            {
                bEffectOn = true;
                card.elementTypes[BulletElementType.Electric] = data;
            }
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var card = complexSystemActionCommandHandler.GetCurrentInherenceCard();

        if (bUpgraded == false)
        {
            if (bEffectOn)
            {
                if (card.elementTypes[BulletElementType.Electric].nestingCnt > 1)
                {
                    var data = card.elementTypes[BulletElementType.Electric];
                    data.nestingCnt -= 1;
                    card.elementTypes[BulletElementType.Electric] = data;
                }
                else
                {
                    card.elementTypes.Remove(BulletElementType.Electric);
                }
            }
        }
        else
        {
            if (bUpgradedEffectOn)
            {
                if (card.debuffTypes[DebuffElementEffectType.ElectricShock].turnCnt > 2)
                {
                    var data = card.debuffTypes[DebuffElementEffectType.ElectricShock];
                    data.turnCnt -= 2;
                    card.debuffTypes[DebuffElementEffectType.ElectricShock] = data;
                }
                else
                {
                    card.debuffTypes.Remove(DebuffElementEffectType.ElectricShock);
                }
            }
            else
            {
                if (bEffectOn)
                {
                    if (card.elementTypes[BulletElementType.Electric].nestingCnt > 1)
                    {
                        var data = card.elementTypes[BulletElementType.Electric];
                        data.nestingCnt -= 1;
                        card.elementTypes[BulletElementType.Electric] = data;
                    }
                    else
                    {
                        card.elementTypes.Remove(BulletElementType.Electric);
                    }
                }
            }
        }
    }
}