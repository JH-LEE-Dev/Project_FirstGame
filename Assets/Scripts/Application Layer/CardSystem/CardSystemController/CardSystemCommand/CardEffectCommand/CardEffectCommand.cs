using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class CardEffectCommand : CardSystemCommand
{
    //이벤트
    public event Action EffectCanApplyEvent;

    //외부 의존성
    protected ICardEffectData cardEffectData;

    [SerializeField] private CardEffectCommand followUpEffectCommand_Prefab;
    protected CardEffectCommand followUpEffectCommand;

    public bool bUpgraded = false;
    public bool bEffectApplied = false;
    protected float valueModifier = 1f;
    protected int condition = -1;

    public IReadOnlyDictionary<BulletElementType, BulletElementData> elementTypes;
    public IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffTypes;

    public virtual void InitializeCommand(ICardEffectData _cardEffectData,
        GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        cardEffectData = _cardEffectData;
        gameSystemActionContext = _cardSystemContextType;
        bUpgraded = cardEffectData.bUpgraded;
        elementTypes = cardEffectData.elementTypes;
        debuffTypes = cardEffectData.debuffTypes;

        if (followUpEffectCommand_Prefab != null && followUpEffectCommand == null)
        {
            followUpEffectCommand = Instantiate(followUpEffectCommand_Prefab);
            followUpEffectCommand.InitializeCommand(cardEffectData, _cardSystemContextType);
        }
    }

    protected void CheckApplyCondition()
    {
        EffectCanApplyEvent?.Invoke();
        EffectCanApplyEvent = null;
    }

    public virtual void ResetCommandData()
    {
        condition = -1;
        valueModifier = 1f;
    }

    public abstract bool EffectConditionCheck();
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