using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/RiftDetection")]
public class EffectCommand_RiftDetection : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int weaknessTurn = 0;
    [SerializeField] private int bonusAttack = 0;

    [SerializeField] private int upgradedWeaknessTurn = 0;
    [SerializeField] private int upgradedBonusAttack = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(bonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(weaknessTurn * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedBonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(upgradedWeaknessTurn * valueModifier);
        }

        ResetCommandData();
    }

    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-bonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-weaknessTurn * valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-upgradedBonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-upgradedWeaknessTurn * valueModifier);
        }
    }
}
