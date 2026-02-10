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
        cardStatusEffectCommandHandler.SetBulletType(BulletType.ArcDischarge, bUpgraded);

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
