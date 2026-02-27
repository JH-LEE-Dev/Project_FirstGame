using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/MeteorShower")]
public class EffectCommand_MeteorShower : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int bonusAttack = 0;
    [SerializeField] private int upgradedBonusAttack = 0;

    public override bool EffectConditionCheck()
    {
        CalcValueModifier();

        int newCondition = 0;

        if (newCondition != condition)
        {
            CheckApplyCondition();
            condition = newCondition;
        }
        return true;
    }

    private void CalcValueModifier()
    {
        if (cardEffectData.effectModifiers.ContainsKey(EffectModType.AllValueModifier))
        {
            valueModifier = cardEffectData.effectModifiers[EffectModType.AllValueModifier].value;
        }
    }

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        EffectConditionCheck();

        var handPile = handler.cardLogicSystem.GetHandPile();

        if (bUpgraded == false)
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(bonusAttack * valueModifier);
        }
        else
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(upgradedBonusAttack * valueModifier);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        if (bUpgraded == false)
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(-bonusAttack * valueModifier);
        }
        else
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(-upgradedBonusAttack * valueModifier);
        }
    }
}