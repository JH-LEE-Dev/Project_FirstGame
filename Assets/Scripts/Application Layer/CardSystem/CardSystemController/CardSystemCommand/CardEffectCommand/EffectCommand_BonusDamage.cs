using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] float bonusDamage = 0f;
    [SerializeField] float upgradedBonusDamage = 0f;

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
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(bonusDamage * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusDamage * valueModifier);
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-bonusDamage * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusDamage * valueModifier);
        }
    }
}