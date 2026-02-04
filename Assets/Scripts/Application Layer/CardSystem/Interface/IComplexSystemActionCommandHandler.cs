using System;
using System.Collections.Generic;

public interface IComplexSystemActionCommandHandler : ICommandHandler
{
    void ApplyAttackCntModifier(int attckCnt,CardSystemContextType cardSystemContextType);
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCards();
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentBulletCards();
    void GraveCardsToHand(ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    IReadOnlyList<CardDataInstance> GetHandPile();
    IReadOnlyList<CardDataInstance> GetDeckPile();
    IReadOnlyList<CardDataInstance> GetGravePile();
    IReadOnlyList<CardDataInstance> GetExtinctionPile();
    void CardPileUse(ReadOnlySpan<CardDataInstance> cardPile, CardSystemContextType cardSystemContextType);
    void UndoCardPileUse(ReadOnlySpan<CardDataInstance> cardPile, CardSystemContextType cardSystemContextType);
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile, CardSystemContextType cardSystemContextType);
    void ApplyAttackModifier(int attack, CardSystemContextType cardSystemContextType);
    int GetPrevUsedBulletCardCnt();
    int GetPrevUsedCardCnt();
    void AdditionalDraw(int amount, CardSystemContextType cardSystemContextType);
    void StartCardSelectionMode(SelectCardPileType selectCardPileType, CardSelectionMode cardSelectionMode, int amount, CardSystemContextType cardSystemContextType,IReadOnlyList<ICardDataInstanceProvider> forbiddenCards,bool _bForced, Action<List<ICardDataInstanceProvider>> onComplete);
    void RequestCardSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType, CardSystemActionTimingType _type = CardSystemActionTimingType.Instant);
    void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType, CardSystemActionTimingType _type = CardSystemActionTimingType.Instant);
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant, CardSystemContextType cardSystemContextType);
    void RevertCardsUpgrade(ReadOnlySpan<CardDataInstance> cards, bool bPermenant, CardSystemContextType cardSystemContextType);
    void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier, CardSystemContextType cardSystemContextType);
    void UndoValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier, CardSystemContextType cardSystemContextType);
    void CardsRemoveFromHands(ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    void ExtinctionCardsToDeck(ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
    IReadOnlyList<CardDataInstance> GetPrevHandToGraveCards();
    void ApplyCardUsePhaseCntModifier(int cnt, CardSystemContextType cardSystemContextType);
    void ExecuteHandPileExistEffect(ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType);
}
