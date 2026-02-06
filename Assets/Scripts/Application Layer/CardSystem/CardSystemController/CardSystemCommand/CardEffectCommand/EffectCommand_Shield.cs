using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Shield")]
public class EffectCommand_Shield : CardEffectCommand<IStatusEffectCommandHandler>
{
    [SerializeField] float bonusShield = 0f;
    [SerializeField] float upgradedBonusShield = 0f;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.ApplyShieldModifier(bonusShield  * valueModifier);
        else
            cardStatusEffectCommandHandler.ApplyShieldModifier(upgradedBonusShield  * valueModifier);


        ResetCommandData();
    }
    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}