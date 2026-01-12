using UnityEngine;

public class CardEffectSystemCommand : CardEffectCommand
{
    [SerializeField] protected CardSystemActionTimingType cardSystemActionTimingType;

    public CardSystemActionTimingType GetCardSystemActionTimingType()
    {
        return cardSystemActionTimingType;
    }

    public virtual void Execute(ICardEffectCommandHandler cardEffectCommandHandler) { }
}
