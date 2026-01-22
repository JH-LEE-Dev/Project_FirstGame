using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/AttackAgain")]
public class EffectCommand_AttackAgain : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int attackCnt = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyAttackCntModifier(attackCnt);

        ResetCommandData();
    }
}
