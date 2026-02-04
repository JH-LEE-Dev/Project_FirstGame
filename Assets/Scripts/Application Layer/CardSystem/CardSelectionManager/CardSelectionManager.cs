using UnityEngine;
using System;
using System.Collections.Generic;

public class CardSelectionManager : ICardSelectionSystemActionCommandHandler
{
    public event Action<CardSelectionModeData> CardSelectionStartEvent;
    public delegate void RequestCardSystemActionDelegate(CardLogicSystemActionType type, ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType,CardSystemActionTimingType cardSystemActionTimingType = CardSystemActionTimingType.Instant);
    public RequestCardSystemActionDelegate RequestCardLogicSystemActionEvent;
    public delegate void RequestCardDataControlSystemActionDelegate(CardDataControlSystemActionType type, ReadOnlySpan<CardDataInstance> cards, CardSystemContextType cardSystemContextType, CardSystemActionTimingType cardSystemActionTimingType = CardSystemActionTimingType.Instant);
    public RequestCardDataControlSystemActionDelegate RequestCardDataControlSystemActionEvent;

    private CardSelectionMode cardSelectionMode;
    private SelectCardPileType selectCardPileType;

    Action<List<ICardDataInstanceProvider>> onCompleteAction;

    public void StartCardSelectionMode(SelectCardPileType _selectCardPileType, CardSelectionMode _cardSelectionMode, int amount, IReadOnlyList<ICardDataInstanceProvider> _forbiddenCards,bool _bForced,
        Action<List<ICardDataInstanceProvider>> onComplete)
    {
        selectCardPileType = _selectCardPileType;
        cardSelectionMode = _cardSelectionMode;
        onCompleteAction = onComplete;
        CardSelectionModeData data = new CardSelectionModeData(selectCardPileType,cardSelectionMode, amount, _forbiddenCards, _bForced);

        CardSelectionStartEvent?.Invoke(data);
    }

    public void ExecuteCommand(CardSystemCommand command,bool bUndo)
    {
        if (bUndo == false)
            command.Execute(this);
        else
            command.Undo(this);
    }

    public void CardSelectionEnd(CardSelectionModeData _data, List<ICardDataInstanceProvider> _cards)
    {
        onCompleteAction?.Invoke(_cards);
    }

    public void SetCardSystemContext(CardSystemContextType cardSystemContextType)
    {
        
    }
}
