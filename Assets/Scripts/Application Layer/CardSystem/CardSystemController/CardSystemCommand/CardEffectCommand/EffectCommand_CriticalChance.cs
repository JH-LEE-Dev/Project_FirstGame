using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/CriticalChance")]
public class EffectCommand_CriticalChance : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int bonusChance = 0;
    [SerializeField] private int upgradedBonusChance = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusChance * valueModifier * nestingCnt);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(upgradedBonusChance * valueModifier * upgradeNestingCnt);

        ResetCommandData();
    }
    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-bonusChance * valueModifier * nestingCnt);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-upgradedBonusChance * valueModifier * upgradeNestingCnt);

        ResetCommandData();
    }
}
