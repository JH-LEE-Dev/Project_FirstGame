using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Hand Enhancement")]
public class EffectCommand_HandEnhancement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int upgradeAmount = 1;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        if (nestingCnt != 0)
        {
            if(complexSystemActionCommandHandler.GetHandPile().Count > upgradeAmount)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand, CardSelectionMode.UpgradeCardsToHand, upgradeAmount * nestingCnt * valueModifier);
            else
            {
                for (int i = 0; i < handPile.Count; ++i)
                {
                    handPile[i].bUpgrade = true;
                }
            }
        }

        if (upgradeNestingCnt != 0)
        {
            for (int i = 0; i < handPile.Count; ++i)
            {
                handPile[i].bUpgrade = true;
            }
        }

        ResetCommandData();
    }
}