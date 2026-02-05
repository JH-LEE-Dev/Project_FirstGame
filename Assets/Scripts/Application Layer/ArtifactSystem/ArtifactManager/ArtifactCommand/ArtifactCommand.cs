using UnityEngine;

public abstract class ArtifactCommand : GameSystemCommand
{
    public int valueModifier = 1;
    public bool bUpgraded = false;

    public virtual void InitializeCommand(int _valueModifier, bool _bUpgraded, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        gameSystemActionContext = _cardSystemContextType;
        valueModifier = _valueModifier;
        bUpgraded = _bUpgraded;
    }
}

public abstract class ArtifactCommand<THandler> : ArtifactCommand
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