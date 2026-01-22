using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Shield")]
public class EffectCommand_Shield : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusShield = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyShieldModifier(bonusShield);

        ResetCommandData();
    }
}