using System;
using UnityEditor;
using UnityEngine;

public abstract class CardSystemCommand : ScriptableObject, ICardSystemActionCommand, ICardStatusEffectCommand
{
    protected CardSystemContextType cardSystemContextType = CardSystemContextType.MAX;

    [SerializeField] protected CardSystemActionTimingType cardSystemActionTimingType;

    public bool IsActive { get; private set; }

    public abstract void Execute(ICommandHandler handler);

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public CardSystemActionTimingType GetCardActionTimingType()
    {
        return cardSystemActionTimingType;
    }

    public CardSystemContextType GetCardSystemContext()
    {
        return cardSystemContextType;
    }
}

/*public class CardSystemCommand : ScriptableObject, ICardStatusEffectCommand, ICardSystemActionCommand
{
    // 현재 명령이 실행 중인지 여부
    public bool IsActive { get; private set; }

    [SerializeField] protected CardSystemActionTimingType cardSystemActionTimingType;

    public virtual void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler) { }

    public virtual void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler) { }

    public virtual void Execute(ICardSlotSystemActionCommandHandler  cardSlotSystemActionCommandHandler) { }

    public virtual void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler,
        ICardStatusEffectCommandHandler cardStatusEffectCommandHandler) { }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public CardSystemActionTimingType GetCardActionTimingType()
    {
        return cardSystemActionTimingType;
    }
}*/
