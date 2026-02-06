using System;
using System.Collections.Generic;

public interface ICardLogicSystemActionCommandHandler : ICommandHandler
{
    void SetCardSystemContext(GameSystemActionContextType cardSystemContextType);
    IReadOnlyList<CardDataInstance> GetHandPile();
    IReadOnlyList<CardDataInstance> GetExtinctionPile();
    IReadOnlyList<CardDataInstance> GetDeckPile();
    IReadOnlyList<CardDataInstance> GetGravePile();
    void StartCardPileDraw();
    void DrawAgain(int drawAmount);
    void GraveCardsToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards);
    void CardsToGrave(ReadOnlySpan<CardDataInstance> cards);
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cards);
    CardDataInstance CreateCard(int id);
    void CardsRemoveFromHand(ReadOnlySpan<CardDataInstance> cards);
    void ResetCardPiles();
    void ExtinctionCardsToDeck(ReadOnlySpan<CardDataInstance> cards);
    void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards);
    void CardsToHand(ReadOnlySpan<CardDataInstance> cards);
    void CardsToDeck(ReadOnlySpan<CardDataInstance> cards);
    void HandToGrave();
    void AddCardsToDeck(ReadOnlySpan<CardDataInstance> _cards);
    void DeleteCardsFromDeck(ReadOnlySpan<CardDataInstance> _cards);
}
