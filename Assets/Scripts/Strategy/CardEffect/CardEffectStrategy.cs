using UnityEngine;

public abstract class CardEffectStrategy : ScriptableObject
{
    protected ICardLogicSystem cardLogicSystem;
    protected IUnitLogicSystemProvider unitLogicSystem;

    [SerializeField] protected CardEffectApplyType effectApplyType;
    protected bool bUpgrade = false;

    public void Initialize(ICardLogicSystem _cardLogicSystem,IUnitLogicSystemProvider _unitLogicSystem)
    {
        cardLogicSystem = _cardLogicSystem;
        unitLogicSystem = _unitLogicSystem;
    }

    public abstract void Execute_Status();
    public abstract void Execute_System();

    public void SetbUpgrade(bool boolean)
    {
        bUpgrade = boolean;
    }

    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }
}
