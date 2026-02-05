using System;
using System.Collections.Generic;

public interface IComplexSystemActionCommandHandler : ICommandHandler
{
    void ApplyAttackCntModifier(int attckCnt,GameSystemActionContextType cardSystemContextType);
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCards();
    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetCurrentBulletCards();
    void GraveCardsToHand(ReadOnlySpan<CardDataInstance> cards, GameSystemActionContextType cardSystemContextType);
    void GraveCardsToDeck(ReadOnlySpan<CardDataInstance> cards, GameSystemActionContextType cardSystemContextType);
    IReadOnlyList<CardDataInstance> GetHandPile();
    IReadOnlyList<CardDataInstance> GetDeckPile();
    IReadOnlyList<CardDataInstance> GetGravePile();
    IReadOnlyList<CardDataInstance> GetExtinctionPile();
    void UseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> cardPile, GameSystemActionContextType cardSystemContextType);
    void UndoCardPileUse(ReadOnlySpan<CardDataInstance> cardPile, GameSystemActionContextType cardSystemContextType);
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile, GameSystemActionContextType cardSystemContextType);
    void ApplyAttackModifier(int attack, GameSystemActionContextType cardSystemContextType);
    int GetPrevUsedBulletCardCnt();
    int GetPrevUsedCardCnt();
    void AdditionalDraw(int amount, GameSystemActionContextType cardSystemContextType);
    void StartCardSelectionMode(SelectCardPileType selectCardPileType, CardSelectionMode cardSelectionMode, int amount, GameSystemActionContextType cardSystemContextType,IReadOnlyList<ICardDataInstanceProvider> forbiddenCards,bool _bForced, Action<List<ICardDataInstanceProvider>> onComplete);
    void RequestCardSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType, GameSystemActionTimingType _type = GameSystemActionTimingType.Instant);
    void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType, GameSystemActionTimingType _type = GameSystemActionTimingType.Instant);
    void UpgradeCards(ReadOnlySpan<CardDataInstance> cards, bool bPermenant, GameSystemActionContextType cardSystemContextType);
    void RevertCardsUpgrade(ReadOnlySpan<CardDataInstance> cards, bool bPermenant, GameSystemActionContextType cardSystemContextType);
    void ApplyValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier, GameSystemActionContextType cardSystemContextType);
    void UndoValueModifier(ReadOnlySpan<CardDataInstance> cards, int valueModifier, GameSystemActionContextType cardSystemContextType);
    void CardsRemoveFromHands(ReadOnlySpan<CardDataInstance> cards, GameSystemActionContextType cardSystemContextType);
    void ExtinctionCardsToDeck(ReadOnlySpan<CardDataInstance> cards, GameSystemActionContextType cardSystemContextType);
    IReadOnlyList<CardDataInstance> GetPrevHandToGraveCards();
    void ApplyCardUsePhaseCntModifier(int cnt, GameSystemActionContextType cardSystemContextType);
    void ExecuteHandPileExistEffect(ReadOnlySpan<CardDataInstance> cards, GameSystemActionContextType cardSystemContextType);
}
