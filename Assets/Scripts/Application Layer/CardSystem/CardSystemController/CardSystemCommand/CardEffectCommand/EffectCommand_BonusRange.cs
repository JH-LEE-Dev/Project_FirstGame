using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusRange")]
public class EffectCommand_BonusRange : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private float upgradedBonusRange = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyRangeModifier(bonusRange * valueModifier * nestingCnt);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyRangeModifier(upgradedBonusRange * valueModifier * upgradeNestingCnt);

        ResetCommandData();
    }
    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyRangeModifier(-bonusRange * valueModifier * nestingCnt);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyRangeModifier(-upgradedBonusRange * valueModifier * upgradeNestingCnt);

        ResetCommandData();
    }
}
