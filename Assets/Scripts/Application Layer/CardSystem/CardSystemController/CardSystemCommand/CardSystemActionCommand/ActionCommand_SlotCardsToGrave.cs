using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardLogicSystemAction/SlotCardsToGrave")]
public class ActionCommand_SlotCardsToGrave : CardSystemActionCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_cards, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.SlotCardsToGrave;
    }

    protected override void Execute(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        cardSystemActionCommandHandler.CardsToGrave(cards.AsSpan<CardDataInstance>().Slice(0, cnt));
    }
    protected override void Undo(ICardLogicSystemActionCommandHandler cardSystemActionCommandHandler)
    {

    }
}
