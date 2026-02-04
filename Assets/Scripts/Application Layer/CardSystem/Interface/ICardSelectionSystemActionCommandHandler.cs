using System;
using System.Collections.Generic;

public interface ICardSelectionSystemActionCommandHandler : ICommandHandler
{
    void StartCardSelectionMode(SelectCardPileType _selectCardPileType, CardSelectionMode _cardSelectionMode, int amount, IReadOnlyList<ICardDataInstanceProvider> _forbiddenCards, bool _bForced,
        Action<List<ICardDataInstanceProvider>> onComplete);
}
