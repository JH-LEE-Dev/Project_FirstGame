using System.Collections.Generic;
using System;

public interface IStatusEffectCommandHandler : ICommandHandler
{
    void ApplyShieldModifier(float bonusShield);
    void ApplyAdditionalAttackModifier(float bonusDamage);
    void ApplyAttackModifier(float bonusDamage);
    void ApplyRangeModifier(float bonusRange);  
    void ApplyAttackCntModifier(int cnt);
    void ApplyCriticalChanceModifier(int chance);
    void ApplyWeaknessModifier(int turnCnt);
    void ApplyAdditionalAttackValueModifier(float bonusDamage);
    void ApplyTotalDamageValueModifier(float bonusValue);
    void UndoAdditionalAttackValueModifier(float bonusDamage);
    void HPDecrease(float amount);
    void HPIncrease(float amount);
    void SetCharacterCanAttackState(bool bCanAttack);
    void ApplyBulletElementType(BulletElementData effectElementData);
    void SetBulletType(BulletType bulletType, bool bUpgraded);
    void ResetBulletType();
    void UndoBulletElementApply(BulletElementData _effectElementData);
    void ApplyDebuffElementType(DebuffElementData _debuffElementData);
    void UndoDebuffElementApply(DebuffElementData _debuffElementData);

    public event Action<ElementExplosionType> ElementExplosionOccuredEvent;
    IPlayerHandler GetPlayerHandler();
    IReadOnlyList<IEnemyHandler> GetEnemyHandlers();
    void ApplyAdditionalAttackStat(AdditionalAttackStat _additionalAttackStat);
    IReadOnlyDictionary<BulletElementType, BulletElementData> GetCurrentAppliedBulletElement();
}
