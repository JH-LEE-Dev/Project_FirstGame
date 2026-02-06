using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/IncreaseHP")]
public class EffectCommand_IncreaseHP : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] float bonusHP = 0f;
    [SerializeField] float upgradedBonusHP = 0f;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.HPIncrease(bonusHP  * valueModifier);
        else
            cardStatusEffectCommandHandler.HPIncrease(upgradedBonusHP  * valueModifier);

        ResetCommandData();
    }
    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}