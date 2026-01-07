using UnityEngine;

public abstract class CardEffectStrategy : ScriptableObject
{
    protected ICardLogicSystem cardLogicSystem;
    protected IUnitLogicSystem unitLogicSystem;

    bool bUpgrade = false;

    public void Initialize(ICardLogicSystem _cardLogicSystem,IUnitLogicSystem _unitLogicSystem)
    {
        cardLogicSystem = _cardLogicSystem;
        unitLogicSystem = _unitLogicSystem;
    }

    public abstract void Execute();
    protected abstract void Execute_Status();
    protected abstract void Execute_System();

    public void SetbUpgrade(bool boolean)
    {
        bUpgrade = boolean;
    }
}
