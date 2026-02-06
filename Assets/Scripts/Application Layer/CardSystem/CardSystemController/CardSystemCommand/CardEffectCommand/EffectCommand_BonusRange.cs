using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusRange")]
public class EffectCommand_BonusRange : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private float upgradedBonusRange = 0;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyRangeModifier(bonusRange * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyRangeModifier(upgradedBonusRange * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyRangeModifier(-bonusRange * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyRangeModifier(-upgradedBonusRange * valueModifier);

        ResetCommandData();
    }
}
