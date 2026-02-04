using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/IncreaseHP")]
public class EffectCommand_IncreaseHP : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusHP = 0f;
    [SerializeField] float upgradedBonusHP = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.HPIncrease(bonusHP * nestingCnt * valueModifier);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.HPIncrease(upgradedBonusHP * upgradeNestingCnt * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}