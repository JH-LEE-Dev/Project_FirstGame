using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Shield")]
public class EffectCommand_Shield : CardEffectCommand
{
    [SerializeField] float bonusShield = 0f;

    public override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.ApplyShieldModifier(bonusShield);
    }
}