using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Inherence/Prism Bolt")]
public class IHEffectCommand_PrismBolt : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private float value = 0;
    [SerializeField] private float attackValue = 0;
    [SerializeField] private float upgradedvalue = 0;
    [SerializeField] private float upgradedAttackValue = 0;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.SetBulletType(BulletType.PrismBolt, bUpgraded);

        BulletElementData data;
        data.bulletElementType = BulletElementType.Electric;

        cardStatusEffectCommandHandler.ApplyBulletElementType(data);

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
        cardStatusEffectCommandHandler.ResetBulletType();

        BulletElementData data;
        data.bulletElementType = BulletElementType.Electric;

        cardStatusEffectCommandHandler.UndoBulletElementApply(data);

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
