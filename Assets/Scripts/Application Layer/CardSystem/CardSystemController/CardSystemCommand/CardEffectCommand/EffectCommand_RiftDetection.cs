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
        if (nestingCnt != 0)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(bonusAttack * valueModifier * nestingCnt);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(weaknessTurn * valueModifier * nestingCnt);
        }

        if (upgradeNestingCnt != 0)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedBonusAttack * valueModifier* upgradeNestingCnt);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(upgradedWeaknessTurn * valueModifier* upgradeNestingCnt);
        }

        ResetCommandData();
    }

    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-bonusAttack * valueModifier * nestingCnt);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-weaknessTurn * valueModifier * nestingCnt);
        }

        if (upgradeNestingCnt != 0)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-upgradedBonusAttack * valueModifier * upgradeNestingCnt);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-upgradedWeaknessTurn * valueModifier * upgradeNestingCnt);
        }
    }
}
