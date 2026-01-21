using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/BonusRange")]
public class EffectCommand_BonusRange : CardEffectCommand
{
    [SerializeField] private float bonusRange = 0;

    public override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyRangeModifier((bonusRange * valueModifier)*(1+nestingCnt));

        ResetCommandData();
    }
}
