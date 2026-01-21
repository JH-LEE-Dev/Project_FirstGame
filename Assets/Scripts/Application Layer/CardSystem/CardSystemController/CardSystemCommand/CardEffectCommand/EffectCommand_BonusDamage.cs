using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectCommand
{
    [SerializeField] float bonusDamage = 0f;

    public override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyAttackModifier((bonusDamage * valueModifier) * (1 + nestingCnt));

        ResetCommandData();
    }
}