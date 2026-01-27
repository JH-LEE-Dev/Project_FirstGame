using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardSystemAction/DuplicateCardsToDeck")]
public class ActionCommand_DuplicateCardsToDeck : CardSystemActionCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.DuplicateCardCardsToHand;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(cnt);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] != null)
            {
                writeBuffer[i] = cardSystemActionCommandHandler.CreateCard(cards[i].GetCardData().id);
            }
        }

        cardSystemActionCommandHandler.CardsToDeck(writeBuffer);

        rentalBuffer.Dispose();
    }
}
