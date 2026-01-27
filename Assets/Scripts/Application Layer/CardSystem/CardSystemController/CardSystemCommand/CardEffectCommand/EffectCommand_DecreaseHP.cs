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
        if (nestingCnt != 0)
            cardStatusEffectCommandHandler.HPDecrease(hpDecreaseAmount * nestingCnt * valueModifier);

        if (upgradeNestingCnt != 0)
            cardStatusEffectCommandHandler.HPDecrease(upgradedHPDecreaseAmount * nestingCnt * valueModifier);


        ResetCommandData();
    }
}