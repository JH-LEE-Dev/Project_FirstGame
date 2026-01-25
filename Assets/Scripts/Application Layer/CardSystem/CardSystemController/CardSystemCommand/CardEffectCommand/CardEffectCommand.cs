using UnityEngine;

public abstract class CardEffectCommand : CardSystemCommand
{
    [SerializeField] protected CardEffectApplyType effectApplyType;

    public int nestingCnt = 0;
    public int upgradeNestingCnt = 0;
    public int valueModifier = 1;

    public void ApplyCardState(int _nestingCnt,int _upgradeNestingCnt,int _valueModifier)
    {
        nestingCnt = _nestingCnt;
        upgradeNestingCnt = _upgradeNestingCnt;
        valueModifier = _valueModifier;
    }

    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }

    public void ResetCommandData()
    {
        nestingCnt = 0;
    }
}

public abstract class CardEffectCommand<THandler> : CardEffectCommand
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