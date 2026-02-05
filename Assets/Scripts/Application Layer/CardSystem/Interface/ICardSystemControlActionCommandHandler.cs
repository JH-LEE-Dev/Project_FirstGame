using System;
using System.Collections.Generic;

public interface ICardSystemControlActionCommandHandler : ICommandHandler
{
    void UseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> usingCards);
    void RequestCardLogicSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType, CardSystemActionTimingType _type = CardSystemActionTimingType.Instant);
    void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType, CardSystemActionTimingType _type = CardSystemActionTimingType.Instant);
    int GetPrevUsedCardCnt();
    void ApplyCardUsePhaseCntModifier(int cnt);
    void ExecuteHandPileExistEffect(ReadOnlySpan<CardDataInstance> cards);
    void UndoUseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> usingCards);
}
