using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusDamage")]
public class EffectCommand_BonusDamage : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusDamage = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyAttackModifier((bonusDamage * valueModifier) * (nestingCnt));

        ResetCommandData();
    }
}