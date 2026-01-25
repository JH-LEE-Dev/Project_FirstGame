using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Distortion")]
public class EffectCommand_Distortion : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;
    [SerializeField] private int bonusCrit = 0;
    [SerializeField] private float bonusDamage = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyRangeModifier(bonusRange);
        cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusCrit);
        cardStatusEffectCommandHandler.ApplyAttackModifier(bonusDamage);

        ResetCommandData();
    }
}
