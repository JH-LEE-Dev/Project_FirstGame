using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/RiftDetection")]
public class EffectCommand_RiftDetection : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private int weaknessTurn = 0;
    [SerializeField] private int bonusAttack = 0;

    [SerializeField] private int upgradedWeaknessTurn = 0;
    [SerializeField] private int upgradedBonusAttack = 0;

    public override bool EffectConditionCheck()
    {
        CalcValueModifier();

        int newCondition = 0;

        if (newCondition != condition)
        {
            CheckApplyCondition();
            condition = newCondition;
        }
        return true;
    }

    private void CalcValueModifier()
    {
        if (cardEffectData.effectModifiers.ContainsKey(EffectModType.AllValueModifier))
        {
            valueModifier = cardEffectData.effectModifiers[EffectModType.AllValueModifier].value;
        }
    }

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        EffectConditionCheck();

        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(bonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(weaknessTurn * (int)valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(upgradedWeaknessTurn * (int)valueModifier);
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-bonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-weaknessTurn * (int)valueModifier);
        }
        else
        {
            cardStatusEffectCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusAttack * valueModifier);
            cardStatusEffectCommandHandler.ApplyWeaknessModifier(-upgradedWeaknessTurn * (int)valueModifier);
        }
    }
}
