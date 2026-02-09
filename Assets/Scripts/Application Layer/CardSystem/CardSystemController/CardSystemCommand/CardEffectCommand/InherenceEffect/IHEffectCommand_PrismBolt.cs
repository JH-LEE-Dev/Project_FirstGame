using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Inherence/Prism Bolt")]
public class IHEffectCommand_PrismBolt : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float value = 0;
    [SerializeField] private float attackValue = 0;
    [SerializeField] private float upgradedvalue = 0;
    [SerializeField] private float upgradedAttackValue = 0;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.SetBulletType(BulletType.PrismBolt, bUpgraded);

        foreach (KeyValuePair<BulletElementType, BulletElementData> pair in elementTypes)
        {
            cardStatusEffectCommandHandler.ApplyBulletElementType(pair.Value);
        }

        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in debuffTypes)
        {
            cardStatusEffectCommandHandler.ApplyDebuffElementType(pair.Value);
        }

        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(attackValue);
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(value);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedAttackValue);
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(upgradedvalue);
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ResetBulletType();


        foreach (KeyValuePair<BulletElementType, BulletElementData> pair in elementTypes)
        {
            cardStatusEffectCommandHandler.UndoBulletElementApply(pair.Value);
        }

        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in debuffTypes)
        {
            cardStatusEffectCommandHandler.UndoDebuffElementApply(pair.Value);
        }


        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-attackValue);
            cardStatusEffectCommandHandler.UndoTotalDamageModifier(value);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-upgradedAttackValue);
            cardStatusEffectCommandHandler.UndoTotalDamageModifier(upgradedvalue);
        }
    }
}
