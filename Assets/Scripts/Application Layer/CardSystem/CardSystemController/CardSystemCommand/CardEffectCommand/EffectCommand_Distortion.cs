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
        if (nestingCnt != 0)
        {
            cardStatusEffectCommandHandler.ApplyRangeModifier(bonusRange * nestingCnt * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusCrit * nestingCnt * valueModifier);
            cardStatusEffectCommandHandler.ApplyAttackModifier(bonusDamage * nestingCnt * valueModifier);
        }

        if(upgradeNestingCnt != 0)
        {
            cardStatusEffectCommandHandler.ApplyRangeModifier(upgradedBonusRange * upgradeNestingCnt * valueModifier);
            cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(upgradedBonusCrit * upgradeNestingCnt * valueModifier);
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedBonusDamage * upgradeNestingCnt * valueModifier);
        }

        ResetCommandData();
    }
}
