using UnityEngine;

public class CardEffectCommand : CardSystemCommand
{
    [SerializeField] protected CardEffectApplyType effectApplyType;

    public int nestingCnt = 0;

    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }

    public void ResetCommandData()
    {
        nestingCnt = 0;
    }
}
