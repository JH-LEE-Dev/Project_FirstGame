using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AttackAgain")]
public class EffectCommand_AttackAgain : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int attackCnt = 0;
    [SerializeField] private int upgradedAttackCnt = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(attackCnt*valueModifier);

        if(upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyAttackCntModifier(upgradedAttackCnt*valueModifier);

        ResetCommandData();
    }
}
