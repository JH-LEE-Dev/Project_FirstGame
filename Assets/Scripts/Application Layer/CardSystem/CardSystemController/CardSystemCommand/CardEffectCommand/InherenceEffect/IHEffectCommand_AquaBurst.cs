using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Inherence/AquaBurst")]
public class IHEffectCommand_AquaBurst : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float value = 1.5f;
    [SerializeField] private float attackValue = 30;
    [SerializeField] private float upgradedvalue = 2;
    [SerializeField] private float upgradedAttackValue = 50;

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

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        EffectConditionCheck();

        AdditionalAttackStat additionalAttackStat;
        DebuffElementData debuffElementData = new DebuffElementData(DebuffElementEffectType.Default, 0);

        if (bUpgraded == false)
        {
            additionalAttackStat = new AdditionalAttackStat(10, 0.5f, 1, debuffElementData);
        }
        else
        {
            additionalAttackStat = new AdditionalAttackStat(20, 1f, 1, debuffElementData);
        }

        cardStatusEffectCommandHandler.SetBulletType(BulletType.AquaBurst, bUpgraded);
        cardStatusEffectCommandHandler.ApplyAdditionalAttackStat(additionalAttackStat);
        cardStatusEffectCommandHandler.SetCharacterCanAttackState(true);

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
        DebuffElementData debuffElementData = new DebuffElementData(DebuffElementEffectType.Default, 0);

        var stat = new AdditionalAttackStat(0, 0, 0, debuffElementData);

        cardStatusEffectCommandHandler.ResetBulletType();
        cardStatusEffectCommandHandler.ApplyAdditionalAttackStat(default);
        cardStatusEffectCommandHandler.SetCharacterCanAttackState(false);

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
