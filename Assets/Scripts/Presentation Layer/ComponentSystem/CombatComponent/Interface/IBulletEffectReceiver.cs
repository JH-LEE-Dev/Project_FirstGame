using UnityEngine;

public interface IBulletEffectReceiver
{
    void SetBulletType(BulletType _type,bool bUpgraded,AdditionalAttackStat _additionalAttackStat);
    void ResetBulletType();
    void ApplyBulletElementType(BulletElementData effectElementData);
    void UndoBulletElementApply(BulletElementData _effectElementData);
    void ApplyDebuffElementType(DebuffElementData _debuffElementData);
    void UndoDebuffElementApply(DebuffElementData _debuffElementData);
}
