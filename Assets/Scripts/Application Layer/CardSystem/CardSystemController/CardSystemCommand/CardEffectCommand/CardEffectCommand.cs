using UnityEngine;

public class CardEffectCommand : CardSystemCommand
{
    [SerializeField] protected CardEffectApplyType effectApplyType;

    public int nestingCnt = 1;

    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }
}
