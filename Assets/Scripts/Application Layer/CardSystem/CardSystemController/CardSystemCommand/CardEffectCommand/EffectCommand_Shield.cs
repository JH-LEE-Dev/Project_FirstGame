using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Shield")]
public class EffectCommand_Shield : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] float bonusShield = 0f;
    [SerializeField] float upgradedBonusShield = 0f;

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
            cardStatusEffectCommandHandler.ApplyShieldModifier(bonusShield  * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyShieldModifier(upgradedBonusShield  * valueModifier);
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}