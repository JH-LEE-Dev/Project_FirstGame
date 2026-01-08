using UnityEngine;

public abstract class CardEffectStrategy : ScriptableObject
{
    protected ICardStrategyHandler cardLogicSystem;
    protected IUnitLogicSystemProvider unitLogicSystem;

    [SerializeField] protected CardEffectApplyType effectApplyType;
    [SerializeField] protected CardSystemActionTimingType cardSystemActionTimingType;
    protected bool bUpgrade = false;

    public void Initialize(ICardStrategyHandler _cardLogicSystem,IUnitLogicSystemProvider _unitLogicSystem)
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

    public CardSystemActionTimingType GetCardSystemActionTimingType()
    {
        return cardSystemActionTimingType;
    }
}
