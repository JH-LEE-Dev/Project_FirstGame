using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Distortion")]
public class EffectCommand_Distortion : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private int bonusCrit = 0;
    [SerializeField] private float bonusDamage = 0;

    [SerializeField] private float upgradedBonusRange = 0;
    [SerializeField] private int upgradedBonusCrit = 0;
    [SerializeField] private float upgradedBonusDamage = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyRangeModifier(bonusRange * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusCrit * valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(bonusDamage * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyRangeModifier(upgradedBonusRange  * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(upgradedBonusCrit  * valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusDamage  * valueModifier);
        }

        ResetCommandData();
    }

    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyRangeModifier(-bonusRange  * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-bonusCrit  * valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-bonusDamage  * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyRangeModifier(-upgradedBonusRange  * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(-upgradedBonusCrit  * valueModifier);
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusDamage  * valueModifier);
        }
    }
}
