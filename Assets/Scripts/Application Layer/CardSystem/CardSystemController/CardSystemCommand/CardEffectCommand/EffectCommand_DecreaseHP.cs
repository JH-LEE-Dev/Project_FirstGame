using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/DecreaseHP")]
public class EffectCommand_DecreaseHP : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int hpDecreaseAmount = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        cardStatusEffectCommandHandler.HPDecrease(hpDecreaseAmount);

        ResetCommandData();
    }
}