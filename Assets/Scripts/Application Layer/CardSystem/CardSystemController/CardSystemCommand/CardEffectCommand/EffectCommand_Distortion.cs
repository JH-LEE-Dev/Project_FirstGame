using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Distortion")]
public class EffectCommand_Distortion : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private int bonusCrit = 0;
    [SerializeField] private float bonusDamage = 0;

    [SerializeField] private float upgradedBonusRange = 0;
    [SerializeField] private int upgradedBonusCrit = 0;
    [SerializeField] private float upgradedBonusDamage = 0;

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

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        EffectConditionCheck();

        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(bonusRange * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusCrit * (int)valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(bonusDamage * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(upgradedBonusRange  * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(upgradedBonusCrit  * (int)valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusDamage  * valueModifier);
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(-bonusRange  * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-bonusCrit  * (int)valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-bonusDamage  * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(-upgradedBonusRange  * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-upgradedBonusCrit  * (int)valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusDamage  * valueModifier);
        }
    }
}
