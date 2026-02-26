using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class CardEffectCommand : CardSystemCommand
{
    public event Action EffectCanApplyEvent;

    [SerializeField] private CardEffectCommand followUpEffectCommand_Prefab;
    protected CardEffectCommand followUpEffectCommand;

    public int valueModifier = 1;
    public bool bUpgraded = false;
    protected bool bCanApply = false;
    public bool bEffectApplied = false;

    public Dictionary<BulletElementType, BulletElementData> elementTypes;
    public Dictionary<DebuffElementEffectType, DebuffElementData> debuffTypes;


    public virtual void InitializeCommand(int _valueModifier,bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes,
        Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes,
        GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        gameSystemActionContext = _cardSystemContextType;
        valueModifier = _valueModifier;
        bUpgraded = _bUpgraded;
        elementTypes = _elementTypes;
        debuffTypes = _debuffTypes;

        if(followUpEffectCommand_Prefab != null && followUpEffectCommand == null)
        {
            followUpEffectCommand = Instantiate(followUpEffectCommand_Prefab);
            followUpEffectCommand.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);
        }
    }

    protected void CheckApplyCondition()
    {
        EffectCanApplyEvent?.Invoke();
    }

    public virtual void ResetCommandData()
    {
        bCanApply = false;
        bEffectApplied = false;
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