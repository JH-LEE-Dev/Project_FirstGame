using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Inherence/PrismBolt")]
public class IHEffectCommand_PrismBolt : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int value = 0;
    [SerializeField] private int upgradedvalue = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(value);
        else
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(upgradedvalue);
    }

    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(value);
        else
            cardStatusEffectCommandHandler.ApplyTotalDamageModifier(upgradedvalue);
    }
}
