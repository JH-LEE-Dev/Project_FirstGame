using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AttackAgain")]
public class EffectCommand_AttackAgain : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int attackCnt = 0;
    [SerializeField] private int upgradedAttackCnt = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(attackCnt * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(upgradedAttackCnt * valueModifier);

        ResetCommandData();
    }

    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(-attackCnt * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(-upgradedAttackCnt * valueModifier);
    }
}
