using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Hand Enhancement")]
public class EffectCommand_HandEnhancement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int upgradeAmount = 1;

    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UpgradeCardsFromHand;
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        if (nestingCnt != 0)
        {
            if(complexSystemActionCommandHandler.GetHandPile().Count > upgradeAmount)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand, CardSelectionMode.UpgradeCardsToHand,
                    upgradeAmount * nestingCnt * valueModifier, cardSystemContextType);
            else
            {
                for (int i = 0; i < handPile.Count; ++i)
                {
                    writeBuffer_Upgrade[i] = handPile[i];
                }

                complexSystemActionCommandHandler.UpgradeCards(writeBuffer_Upgrade,false, cardSystemContextType);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            for (int i = 0; i < handPile.Count; ++i)
            {
                writeBuffer_Upgrade[i] = handPile[i];
            }

            complexSystemActionCommandHandler.UpgradeCards(writeBuffer_Upgrade,false, cardSystemContextType);
        }

        ResetCommandData();
    }
}