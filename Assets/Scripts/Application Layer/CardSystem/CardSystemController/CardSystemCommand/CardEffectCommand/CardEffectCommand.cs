using UnityEngine;

public class CardEffectCommand : CardSystemCommand
{
    [SerializeField] protected CardEffectApplyType effectApplyType;

    public int nestingCnt = 0;
    public int valueModifier = 1;

    public void ApplyCardState(int _nestingCnt,int _valueModifier)
    {
        nestingCnt = _nestingCnt;
        valueModifier = _valueModifier;
    }

    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }

    public void ResetCommandData()
    {
        nestingCnt = 0;
    }
}
