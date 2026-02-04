using System;


public abstract class CardSystemActionCommand : CardSystemCommand
{
    protected CardDataInstance[] cards = new CardDataInstance[SYSTEM_VAR.maxDeckPileCount];
    protected int cnt;

    public virtual void InitializeCommand(ReadOnlySpan<CardDataInstance> _cards,CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        cardSystemContextType = _cardSystemContextType;

        if (cards == null || cards.Length != SYSTEM_VAR.maxDeckPileCount)
        {
            cards = new CardDataInstance[SYSTEM_VAR.maxDeckPileCount];
        }

        cnt = 0;
        for (int i = 0; i < _cards.Length; ++i)
        {
            ++cnt;
            if (_cards[i] != null)
                cards[i] = _cards[i];
        }
    }
}

public abstract class CardSystemActionCommand<THandler> : CardSystemActionCommand
    where THandler : class, ICommandHandler
{
    public override void Execute(ICommandHandler handler)
    {
        if (handler is THandler target)
        {
            Execute(target);
        }
    }

    public override void Undo(ICommandHandler handler)
    {
        if (handler is THandler target)
        {
            Undo(target);
        }
    }

    protected abstract void Execute(THandler handler);
    protected abstract void Undo(THandler handler);
}

