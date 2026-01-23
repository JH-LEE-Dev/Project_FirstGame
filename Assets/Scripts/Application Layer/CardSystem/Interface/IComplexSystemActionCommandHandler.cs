using System;
using System.Collections.Generic;

public interface IComplexSystemActionCommandHandler : ICommandHandler
{
    bool DeckConditionCheck(int cardId);

    void ApplyAttackCntModifier(int attckCnt);

    IReadOnlyList<IReadOnlyList<CardDataInstance>> GetPrevUsedBulletCards();
    void GraveToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards);

    IReadOnlyList<CardDataInstance> GetHandPile();

    void CardPileUse(ReadOnlySpan<CardDataInstance> cardPile);
    void CardsToExtinction(ReadOnlySpan<CardDataInstance> cardPile);
    void ApplyAttackModifier(int attack);
}
