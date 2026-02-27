using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AttackAgain")]
public class EffectCommand_AttackAgain : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] private int attackCnt = 0;
    [SerializeField] private int upgradedAttackCnt = 0;

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
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(attackCnt * (int)valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(upgradedAttackCnt * (int)valueModifier);
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(-attackCnt * (int)valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(-upgradedAttackCnt * (int)valueModifier);
    }
}
