using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/DecreaseHP")]
public class EffectCommand_DecreaseHP : CardEffectCommand<ICardStatusEffectCommandHandler>
{
    [SerializeField] private int hpDecreaseAmount = 0;
    [SerializeField] private int upgradedHPDecreaseAmount = 0;

    protected override void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        if (bUpgraded == false)
            cardStatusEffectCommandHandler.HPDecrease(hpDecreaseAmount * valueModifier);
        else
            cardStatusEffectCommandHandler.HPDecrease(upgradedHPDecreaseAmount * valueModifier);


        ResetCommandData();
    }
    protected override void Undo(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}