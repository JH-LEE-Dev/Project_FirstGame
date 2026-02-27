using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/CriticalChance")]
public class EffectCommand_CriticalChance : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private int bonusChance = 0;
    [SerializeField] private int upgradedBonusChance = 0;

    private void CalcValueModifier()
    {
        if (cardEffectData.effectModifiers.ContainsKey(EffectModType.AllValueModifier))
        {
            valueModifier = cardEffectData.effectModifiers[EffectModType.AllValueModifier].value;
        }
    }


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

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        EffectConditionCheck();

        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusChance * (int)valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(upgradedBonusChance * (int)valueModifier);
    }
    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-bonusChance * (int)valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-upgradedBonusChance * (int)valueModifier);
    }
}
