using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSystemActionCommandHandler : ICommandHandler
{
    void StartCardPileDraw();
    void DrawAgain(int drawAmount);
    void ApplyValueModifier(int valueModifier);

    bool DeckConditionCheck(int cardID);

    void GraveToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards);
    void CardToGrave(CardDataInstance card);
    void CardsToGrave(ReadOnlySpan<CardDataInstance> cards);

    IReadOnlyList<CardDataInstance> GetHandPile();
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile);

    void RandomExtinctionCardToDeck();
    CardDataInstance CreateCard(int id);
}
