using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/QuantumEntanglement")]
public class EffectCommand_QuantumEntanglement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int duplicateAmount = 1;
    [SerializeField] int upgradedDuplicateAmount = 1;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        if (nestingCnt != 0)
        {
            if(handPile.Count > duplicateAmount * nestingCnt * valueModifier)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, duplicateAmount * nestingCnt * valueModifier, cardSystemContextType);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < handPile.Count; ++i)
                {
                    writeBuffer[i] = handPile[i];
                }

                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand,writeBuffer,CardSystemContextType.MAX);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            if (handPile.Count > upgradedDuplicateAmount * upgradeNestingCnt * valueModifier)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, upgradedDuplicateAmount * upgradeNestingCnt * valueModifier, cardSystemContextType);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < handPile.Count; ++i)
                {
                    writeBuffer[i] = handPile[i];
                }

                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer, CardSystemContextType.MAX);
            }
        }

        ResetCommandData();
    }
}