using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/RiftDetection")]
public class EffectCommand_RiftDetection : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private int weaknessTurn = 0;
    [SerializeField] private int bonusAttack = 0;

    [SerializeField] private int upgradedWeaknessTurn = 0;
    [SerializeField] private int upgradedBonusAttack = 0;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(bonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(weaknessTurn * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(upgradedWeaknessTurn * valueModifier);
        }

        ResetCommandData();
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-bonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-weaknessTurn * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-upgradedWeaknessTurn * valueModifier);
        }
    }
}
