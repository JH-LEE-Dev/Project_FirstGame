using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/IncreaseHP")]
public class EffectCommand_IncreaseHP : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] float bonusHP = 0f;
    [SerializeField] float upgradedBonusHP = 0f;

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
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.HPIncrease(bonusHP  * valueModifier);
        else
            cardStatusEffectCommandHandler.HPIncrease(upgradedBonusHP  * valueModifier);
    }
    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}