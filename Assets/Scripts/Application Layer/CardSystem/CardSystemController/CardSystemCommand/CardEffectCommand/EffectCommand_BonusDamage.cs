using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusDamage = 0f;
    [SerializeField] float upgradedBonusDamage = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
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

    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
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