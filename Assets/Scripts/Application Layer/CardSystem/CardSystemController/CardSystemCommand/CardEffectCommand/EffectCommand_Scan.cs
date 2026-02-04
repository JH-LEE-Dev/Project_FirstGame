using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Scan")]
public class EffectCommand_Scan : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int selectCnt = 1;
    [SerializeField] private int upgradedSelectCnt = 2;

    IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        complexSystemActionCommandHandler = _complexSystemActionCommandHandler;

        IReadOnlyList<CardDataInstance> gravePile = complexSystemActionCommandHandler.GetGravePile();

        if (nestingCnt != 0)
        {
            if(gravePile.Count > selectCnt)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Grave,
                    CardSelectionMode.GraveCardsToHand, selectCnt, cardSystemContextType,null,HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0;i<gravePile.Count;++i)
                {
                    writeBuffer[i] = gravePile[i];
                }

                complexSystemActionCommandHandler.GraveCardsToHand(writeBuffer, cardSystemContextType);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            if (gravePile.Count > upgradedSelectCnt)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Grave,
                    CardSelectionMode.GraveCardsToHand, upgradedSelectCnt, cardSystemContextType,null,HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < gravePile.Count; ++i)
                {
                    writeBuffer[i] = gravePile[i];
                }

                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, cardSystemContextType);
            }
        }

        ResetCommandData();
    }

    private void HandleCardSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, cardSystemContextType);
    }
}