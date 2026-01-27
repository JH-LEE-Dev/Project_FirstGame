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
    void CardsToGrave(ReadOnlySpan<CardDataInstance> cards);

    IReadOnlyList<CardDataInstance> GetHandPile();
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cards);

    void RandomExtinctionCardToDeck();
    CardDataInstance CreateCard(int id);
    void CardsRemoveFromHand(ReadOnlySpan<CardDataInstance> cards);
    void ExtinctionToDeck();
    void CardsToHand(ReadOnlySpan<CardDataInstance> cards);
    void CardsToDeck(ReadOnlySpan<CardDataInstance> cards);
}
