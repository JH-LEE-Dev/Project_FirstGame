using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/IncreaseHP")]
public class EffectCommand_IncreaseHP : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] float bonusHP = 0f;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.HPIncrease(bonusHP);

        ResetCommandData();
    }
}