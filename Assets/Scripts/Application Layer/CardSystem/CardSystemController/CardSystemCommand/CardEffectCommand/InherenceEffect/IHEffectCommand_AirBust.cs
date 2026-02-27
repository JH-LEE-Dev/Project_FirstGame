using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Inherence/AirBust")]
public class IHEffectCommand_AirBust : CardEffectCommand<IStatusEffectCommandHandler>
{
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

        cardStatusEffectCommandHandler.SetBulletType(BulletType.PrismBolt, bUpgraded);
        cardStatusEffectCommandHandler.ApplyAdditionalAttackStat(default);
        cardStatusEffectCommandHandler.SetCharacterCanAttackState(true);

        foreach (KeyValuePair<BulletElementType, BulletElementData> pair in elementTypes)
        {
            cardStatusEffectCommandHandler.ApplyBulletElementType(pair.Value);
        }

        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in debuffTypes)
        {
            cardStatusEffectCommandHandler.ApplyDebuffElementType(pair.Value);
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
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
    }
}
