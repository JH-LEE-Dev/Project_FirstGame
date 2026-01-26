using UnityEngine;
using System;

public class CardSelectionManager : ICardSelectionSystemActionCommandHandler
{
    public event Action<CardSelectionModeData> CardSelectionStartEvent;

    private CardSelectionMode cardSelectionMode;

    public void StartCardSelectionMode(CardSelectionMode _cardSelectionMode,int amount)
    {
        CardSelectionModeData data = new CardSelectionModeData(cardSelectionMode,amount );
        cardSelectionMode = _cardSelectionMode;
        CardSelectionStartEvent?.Invoke(data);
    }

    public void ExecuteCommand(CardSystemCommand command)
    {
        command.Execute(this);
    }
}
