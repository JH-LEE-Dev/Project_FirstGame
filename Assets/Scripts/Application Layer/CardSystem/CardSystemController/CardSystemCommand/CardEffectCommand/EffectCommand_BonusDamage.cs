using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] float bonusDamage = 0f;
    [SerializeField] float upgradedBonusDamage = 0f;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(bonusDamage * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusDamage * valueModifier);
        }

        ResetCommandData();
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-bonusDamage * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusDamage * valueModifier);
        }
    }
}