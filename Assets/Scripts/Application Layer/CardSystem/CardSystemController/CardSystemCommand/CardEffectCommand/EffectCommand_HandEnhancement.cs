using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Hand Enhancement")]
public class EffectCommand_HandEnhancement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int upgradeAmount = 1;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
            complexSystemActionCommandHandler.StartCardSelectionMode(CardSelectionMode.UpgradeToHand, upgradeAmount * nestingCnt * valueModifier);

        if (upgradeNestingCnt != 0)
        {
            IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

            for (int i = 0; i < handPile.Count; ++i)
            {
                handPile[i].bUpgrade = true;
            }
        }

        ResetCommandData();
    }
}