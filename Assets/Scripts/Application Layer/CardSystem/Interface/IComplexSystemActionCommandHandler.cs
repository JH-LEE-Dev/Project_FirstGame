using System;
using System.Collections.Generic;

public interface IComplexSystemActionCommandHandler : ICommandHandler
{
    bool DeckConditionCheck(int cardId);

    void ApplyAttackCntModifier(int attckCnt);

    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCards();
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentBulletCards();
    void GraveCardsToHand(ReadOnlySpan<CardDataInstance> cards);
    void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards);

    IReadOnlyList<CardDataInstance> GetHandPile();
    IReadOnlyList<CardDataInstance> GetDeckPile();
    IReadOnlyList<CardDataInstance> GetGravePile();
    IReadOnlyList<CardDataInstance> GetExtinctionPile();

    void CardPileUse(ReadOnlySpan<CardDataInstance> cardPile);
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile);
    void ApplyAttackModifier(int attack);
    int GetPrevUsedBulletCardCnt();
    int GetPrevUsedCardCnt();
    void AdditionalDraw(int amount);
    void StartCardSelectionMode(SelectCardPileType selectCardPileType, CardSelectionMode cardSelectionMode, int amount);
    void RequestCardSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards,CardSystemContextType _cardSystemContextType);
    void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType);
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards);
    void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier);
}
