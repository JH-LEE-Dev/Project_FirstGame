using UnityEngine;

public abstract class CardSystemActionCommand : CardSystemCommand
{
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

    protected abstract void Execute(THandler handler);
}

