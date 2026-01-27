using System;
using System.Collections.Generic;

public interface ICardSystemControlActionCommandHandler
{
    void UseCardsAndExtinguishAll(ReadOnlySpan<CardDataInstance> usingCards);
    void RequestCardSystemActionCommand(CardSystemActionType cardSystemActionType, ReadOnlySpan<CardDataInstance> _cards);
    int GetPrevUsedCardCnt();
}
