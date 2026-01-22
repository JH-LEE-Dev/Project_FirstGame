using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusRange")]
public class EffectCommand_BonusRange : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private float bonusRange = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyRangeModifier((bonusRange * valueModifier)*(1+nestingCnt));

        ResetCommandData();
    }
}
