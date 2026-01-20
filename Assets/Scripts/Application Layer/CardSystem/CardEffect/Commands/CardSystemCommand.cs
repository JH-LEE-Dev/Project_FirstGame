using UnityEngine;

public class CardSystemCommand : ScriptableObject, ICardStatusEffectCommand, ICardSystemActionCommand
{
    // 현재 명령이 실행 중인지 여부
    public bool IsActive { get; private set; }

    [SerializeField] protected CardSystemActionTimingType cardSystemActionTimingType;

    public virtual void Execute(ICardStatusEffectCommandHandler cardStatusEffectCommandHandler) { }

    public virtual void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler) { }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public CardSystemActionTimingType GetCardActionTimingType()
    {
        return cardSystemActionTimingType;
    }
}
