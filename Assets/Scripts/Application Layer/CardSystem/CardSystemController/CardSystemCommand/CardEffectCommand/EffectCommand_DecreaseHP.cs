using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/DecreaseHP")]
public class EffectCommand_DecreaseHP : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private int hpDecreaseAmount = 0;
    [SerializeField] private int upgradedHPDecreaseAmount = 0;

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
            cardStatusEffectCommandHandler.HPDecrease(hpDecreaseAmount * valueModifier);
        else
            cardStatusEffectCommandHandler.HPDecrease(upgradedHPDecreaseAmount * valueModifier);
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}