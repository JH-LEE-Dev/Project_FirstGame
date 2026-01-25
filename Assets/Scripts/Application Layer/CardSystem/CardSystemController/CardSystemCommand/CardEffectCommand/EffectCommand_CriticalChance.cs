using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/CriticalChance")]
public class EffectCommand_CriticalChance : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int bonusChance = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyCriticalChanceModifier(bonusChance * valueModifier * nestingCnt);

        ResetCommandData();
    }
}
