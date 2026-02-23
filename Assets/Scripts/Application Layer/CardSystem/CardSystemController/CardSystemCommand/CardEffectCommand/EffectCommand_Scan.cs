using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Scan")]
public class EffectCommand_Scan : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int selectCnt = 1;
    [SerializeField] private int upgradedSelectCnt = 2;

    IComplexSystemActionCommandHandler handler;

    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> gravePile = handler.cardLogicSystem.GetGravePile();

        for (int i = 0; i < gravePile.Count; ++i)
        {
            availableCards.Add(gravePile[i]);
        }

        if (bUpgraded == false)
        {
            if (availableCards.Count > selectCnt)
                handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Grave,
                    CardSelectionMode.GraveCardsToHand, selectCnt, availableCards, true, HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < availableCards.Count; ++i)
                {
                    writeBuffer[i] = availableCards[i] as CardDataInstance;
                }

                if (availableCards.Count > 0)
                    handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, gameSystemActionContext);
            }
        }
        else
        {
            if (availableCards.Count > upgradedSelectCnt)
                handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Grave,
                    CardSelectionMode.GraveCardsToHand, upgradedSelectCnt, availableCards, true, HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < availableCards.Count; ++i)
                {
                    writeBuffer[i] = availableCards[i] as CardDataInstance;
                }

                if (availableCards.Count > 0)
                    handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, gameSystemActionContext);
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

        if (_cards.Count > 0)
            handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, gameSystemActionContext);
    }
    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}