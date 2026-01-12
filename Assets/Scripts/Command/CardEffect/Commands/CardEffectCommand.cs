using UnityEngine;

public class CardEffectCommand : ScriptableObject
{
    [SerializeField] protected CardEffectApplyType effectApplyType;

    protected bool bUpgrade = false;

    public void SetbUpgrade(bool boolean)
    {
        bUpgrade = boolean;
    }

    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }
}
