using System;
using System.Collections.Generic;

public interface ICardSystemControlActionCommandHandler : ICommandHandler
{
    void UseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> usingCards);
    void RequestCardLogicSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType, GameSystemActionTimingType _type = GameSystemActionTimingType.Instant);
    void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType, GameSystemActionTimingType _type = GameSystemActionTimingType.Instant);
    int GetPrevUsedCardCnt();
    void ApplyCardUsePhaseCntModifier(int cnt);
    void ExecuteHandPileExistEffect(ReadOnlySpan<CardDataInstance> cards);
    void UndoUseCards_AfterAttackEffects(ReadOnlySpan<CardDataInstance> usingCards);
}
