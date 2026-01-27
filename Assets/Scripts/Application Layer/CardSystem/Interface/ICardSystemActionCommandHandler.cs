using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSystemActionCommandHandler : ICommandHandler
{
    IReadOnlyList<CardDataInstance> GetHandPile();
    IReadOnlyList<CardDataInstance> GetExtinctionPile();
    IReadOnlyList<CardDataInstance> GetDeckPile();
    IReadOnlyList<CardDataInstance> GetGravePile();

    void StartCardPileDraw();
    void DrawAgain(int drawAmount);
    void ApplyValueModifier(int valueModifier);
    bool DeckConditionCheck(int cardID);
    void GraveCardsToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards);
    void CardsToGrave(ReadOnlySpan<CardDataInstance> cards);
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cards);
    CardDataInstance CreateCard(int id);
    void CardsRemoveFromHand(ReadOnlySpan<CardDataInstance> cards);
    void AllExtinctionCardsToDeck();
    void ExtinctionCardsToDeck(ReadOnlySpan<CardDataInstance> cards);
    void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards);
    void CardsToHand(ReadOnlySpan<CardDataInstance> cards);
    void CardsToDeck(ReadOnlySpan<CardDataInstance> cards);
}
