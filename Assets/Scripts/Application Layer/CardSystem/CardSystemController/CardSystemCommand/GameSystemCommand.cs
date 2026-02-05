using UnityEngine;

public abstract class GameSystemCommand : ScriptableObject
{
    protected GameSystemActionContextType gameSystemActionContext = GameSystemActionContextType.MAX;

    [SerializeField] protected GameSystemActionTimingType gameSystemActionTimingType;

    [SerializeField] protected EffectApplyType effectApplyType;

    public abstract void Execute(ICommandHandler handler);
    public abstract void Undo(ICommandHandler handler);

    public GameSystemActionContextType GetGameSystemContext()
    {
        return gameSystemActionContext;
    }

    public GameSystemActionTimingType GetGameSystemActionTimingType()
    {
        return gameSystemActionTimingType;
    }

    public EffectApplyType GetEffectApplyType()
    {
        return effectApplyType;
    }
}
