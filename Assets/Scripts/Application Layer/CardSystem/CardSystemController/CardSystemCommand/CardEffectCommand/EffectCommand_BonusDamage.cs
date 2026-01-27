using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusDamage = 0f;
    [SerializeField] float upgradedBonusDamage = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyAttackModifier(bonusDamage * valueModifier * nestingCnt);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedBonusDamage * valueModifier * upgradeNestingCnt);

        ResetCommandData();
    }
}