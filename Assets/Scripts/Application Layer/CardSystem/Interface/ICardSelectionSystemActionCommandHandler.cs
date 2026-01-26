using UnityEngine;

public interface ICardSelectionSystemActionCommandHandler : ICommandHandler
{
    void StartCardSelectionMode(CardSelectionMode _cardSelectionMode, int amount);
}
