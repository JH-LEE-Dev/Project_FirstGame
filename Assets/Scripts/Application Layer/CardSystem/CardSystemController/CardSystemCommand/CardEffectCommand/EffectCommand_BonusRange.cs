using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusRange")]
public class EffectCommand_BonusRange : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private float upgradedBonusRange = 0;

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
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(bonusRange * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(upgradedBonusRange * valueModifier);
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(-bonusRange * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyAttackRangeModifier(-upgradedBonusRange * valueModifier);
    }
}
