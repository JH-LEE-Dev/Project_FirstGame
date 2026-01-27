using UnityEngine;

public interface ICardSelectionSystemActionCommandHandler : ICommandHandler
{
    void StartCardSelectionMode(SelectCardPileType _selectCardPileType, CardSelectionMode _cardSelectionMode, int amount);
}
