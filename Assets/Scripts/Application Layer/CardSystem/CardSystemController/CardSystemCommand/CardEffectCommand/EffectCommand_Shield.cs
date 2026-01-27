using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Shield")]
public class EffectCommand_Shield : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusShield = 0f;
    [SerializeField] float upgradedBonusShield = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyShieldModifier(bonusShield * nestingCnt * valueModifier);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.ApplyShieldModifier(upgradedBonusShield * upgradeNestingCnt * valueModifier);


        ResetCommandData();
    }
}