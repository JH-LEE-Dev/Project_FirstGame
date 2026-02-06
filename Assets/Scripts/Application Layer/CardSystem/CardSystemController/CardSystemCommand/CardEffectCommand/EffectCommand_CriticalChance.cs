using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/CriticalChance")]
public class EffectCommand_CriticalChance : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private int bonusChance = 0;
    [SerializeField] private int upgradedBonusChance = 0;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusChance * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(upgradedBonusChance * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-bonusChance * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-upgradedBonusChance * valueModifier);

        ResetCommandData();
    }
}
