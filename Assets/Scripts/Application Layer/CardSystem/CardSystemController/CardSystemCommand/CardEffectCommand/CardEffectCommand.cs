using UnityEngine;

public abstract class CardEffectCommand : CardSystemCommand
{
    [SerializeField] protected CardEffectApplyType effectApplyType;

    public int nestingCnt = 0;
    public int upgradeNestingCnt = 0;
    public int valueModifier = 1;

    public virtual void InitializeCommand(int _nestingCnt,int _upgradeNestingCnt,int _valueModifier,CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        cardSystemContextType = _cardSystemContextType;
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
        //nestingCnt = 0;
        //upgradeNestingCnt = 0;
        //valueModifier = 1;
        //cardSystemContextType = CardSystemContextType.MAX;
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