using System;
using System.Collections.Generic;

public interface ICardSelectionSystemActionCommandHandler : ICommandHandler
{
    void StartCardSelectionMode(SelectCardPileType _selectCardPileType, CardSelectionMode _cardSelectionMode, int amount,List<CardName> _forbiddenCards,
        Action<List<ICardDataInstanceProvider>> onComplete);
}
