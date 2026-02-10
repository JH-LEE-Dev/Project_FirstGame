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
        AdditionalAttackStat additionalAttackStat;

        if (bUpgraded)
        {
            additionalAttackStat = new AdditionalAttackStat(5, 0.5f, 1,default);
        }
        else
        {
            additionalAttackStat = new AdditionalAttackStat(2, 0.2f, 1, default);
        }

        cardStatusEffectCommandHandler.SetBulletType(BulletType.PrismBolt, bUpgraded,additionalAttackStat);

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
            cardStatusEffectCommandHandler.ApplyAdditionalAttackValueModifier(value);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedAttackValue);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackValueModifier(upgradedvalue);
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
            cardStatusEffectCommandHandler.UndoAdditionalAttackValueModifier(value);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-upgradedAttackValue);
            cardStatusEffectCommandHandler.UndoAdditionalAttackValueModifier(upgradedvalue);
        }
    }
}
