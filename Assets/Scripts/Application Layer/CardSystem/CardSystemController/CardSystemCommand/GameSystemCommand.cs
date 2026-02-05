using UnityEngine;

public abstract class GameSystemCommand : ScriptableObject
{
    protected GameSystemActionContextType gameSystemActionContext = GameSystemActionContextType.MAX;

    [SerializeField] protected GameSystemActionTimingType gameSystemActionTimingType;

    public abstract void Execute(ICommandHandler handler);
    public abstract void Undo(ICommandHandler handler);

    public GameSystemActionContextType GetCardSystemContext()
    {
        return gameSystemActionContext;
    }

    public GameSystemActionTimingType GetGameSystemActionTimingType()
    {
        return gameSystemActionTimingType;
    }
}
