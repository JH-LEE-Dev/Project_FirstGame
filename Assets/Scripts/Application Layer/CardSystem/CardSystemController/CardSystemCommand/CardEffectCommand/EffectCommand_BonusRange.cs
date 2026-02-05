using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusRange")]
public class EffectCommand_BonusRange : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private float upgradedBonusRange = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyRangeModifier(bonusRange * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyRangeModifier(upgradedBonusRange * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyRangeModifier(-bonusRange * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyRangeModifier(-upgradedBonusRange * valueModifier);

        ResetCommandData();
    }
}
