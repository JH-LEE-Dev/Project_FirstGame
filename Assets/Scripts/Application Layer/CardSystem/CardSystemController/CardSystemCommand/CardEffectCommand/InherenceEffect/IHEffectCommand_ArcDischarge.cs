using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Inherence/Arc Discharge")]
public class IHEffectCommand_ArcDischarge : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float value = 1.5f;
    [SerializeField] private float attackValue = 30;
    [SerializeField] private float upgradedvalue = 2;
    [SerializeField] private float upgradedAttackValue = 50;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(attackValue);
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(value);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(upgradedAttackValue);
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(upgradedvalue);
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-attackValue);
            cardStatusEffectCommandHandler.UndoTotalDamageModifier(value);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAttackModifier(-upgradedAttackValue);
            cardStatusEffectCommandHandler.UndoTotalDamageModifier(upgradedvalue);
        }
    }
}
