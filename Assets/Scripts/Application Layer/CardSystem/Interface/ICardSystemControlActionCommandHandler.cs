using System;
using System.Collections.Generic;

public interface ICardSystemControlActionCommandHandler
{
    void UseCardsAndExtinguishAll(ReadOnlySpan<CardDataInstance> usingCards);
    void RequestCardLogicSystemActionCommand(CardLogicSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType);
    void RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType cardDataControlSystemActionType, ReadOnlySpan<CardDataInstance> _cards, CardSystemContextType _cardSystemContextType);
    int GetPrevUsedCardCnt();
}
