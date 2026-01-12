using UnityEngine;

public class CardEffectCommand : ScriptableObject
{
    // 현재 명령이 실행 중인지 여부
    public bool IsActive { get; private set; }

    [SerializeField] protected CardEffectApplyType effectApplyType;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;


    public CardEffectApplyType GetCardEffectApplyType()
    {
        return effectApplyType;
    }
}
