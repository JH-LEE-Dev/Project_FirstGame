using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Earth : MonoBehaviour, IDamageable, IPlayerData, IPlayerHandler
{
    public event Action<float> TakeDamageEvent;
    public event Action PlayerDeadEvent;
    public event Action PlayerDebuffChangedEvent;
    public event Action<IPlayerData, Vector2, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData>> PlayerHitEvent;

    //인터페이스 선언부
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> IPlayerData.currentAppliedDebuff => currentAppliedDebuff;
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> IPlayerHandler.currentAppliedDebuff => currentAppliedDebuff;


    private Dictionary<DebuffElementEffectType, DebuffElementData> currentAppliedDebuff = new Dictionary<DebuffElementEffectType, DebuffElementData>(SYSTEM_VAR.maxDebuffElementCount);
    public IStatusEffectReceiver statusEffectReceiver => healthComponent;


    protected HealthComponent healthComponent;
    protected ElementDamageHandleComponent elementDamageHandleComponent;

    private int money = 0;

    private void Awake()
    {

    }

    public void Initialize()
    {
        healthComponent = GetComponent<HealthComponent>();
        elementDamageHandleComponent = new ElementDamageHandleComponent();

        elementDamageHandleComponent.Initialize(currentAppliedDebuff);

        BindEvents();
    }

    private void BindEvents()
    {
        healthComponent.UnitIsDeadEvent -= PlayerIsDead;
        healthComponent.UnitIsDeadEvent += PlayerIsDead;
    }

    private void ReleaseEvents()
    {
        healthComponent.UnitIsDeadEvent -= PlayerIsDead;
    }

    private void PlayerIsDead()
    {
        PlayerDeadEvent?.Invoke();
    }

    private void OnDestroy()
    {
        TakeDamageEvent = null;
        ReleaseEvents();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetMaxHealth()
    {
        return healthComponent.GetMaxHealth();
    }

    public float GetCurrentHealth()
    {
        return healthComponent.GetCurrentHealth();
    }

    public float GetCurrentShield()
    {
        return healthComponent.GetCurrentShield();
    }

    public float GetPrevHealth()
    {
        return healthComponent.GetPrevHealth();
    }

    public float GetPrevShield()
    {
        return healthComponent.GetPrevShield();
    }

    public void KnockBack(Vector2 dir, float power)
    {
        return;
    }

    public void ApplyWeakness(int turnCnt)
    {
        throw new NotImplementedException();
    }

    public void ResetPlayer()
    {
        healthComponent.ResetHealthComponent();
    }

    public void ResetShield()
    {
        healthComponent.ResetShield();
    }

    public void EarnMoney(int amount)
    {
        money += amount;
    }

    public void UseMoney(int amount)
    {
        money -= amount;
    }

    public int GetPlayerCurrentMoney()
    {
        return money;
    }

    public void ClearDebuff()
    {
        currentAppliedDebuff.Clear();

        PlayerDebuffChangedEvent?.Invoke();
    }

    public void TakeCollideDamage(float damage, bool bCritical, Vector2 pos, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements = null)
    {
        ApplyElementDebuff(_debuffElements);

        damage = elementDamageHandleComponent.GetResultDamage(_debuffElements, damage);

        healthComponent.TakeDamage(damage);
        TakeDamageEvent?.Invoke(damage);

        if (_debuffElements != null)
            PlayerHitEvent?.Invoke(this, pos, _debuffElements);
    }

    public void PlayerTurnEnd()
    {
        Span<DebuffElementEffectType> allKeys = stackalloc DebuffElementEffectType[currentAppliedDebuff.Count];
        int index = 0;

        foreach (var k in currentAppliedDebuff.Keys)
            allKeys[index++] = k;

        for (int i = 0; i < allKeys.Length; i++)
        {
            var key = allKeys[i];
            var data = currentAppliedDebuff[key];

            if (data.turnCnt <= 1)
            {
                currentAppliedDebuff.Remove(key);
            }
            else
            {
                data.turnCnt -= 1;
                currentAppliedDebuff[key] = data;
            }
        }

        PlayerDebuffChangedEvent?.Invoke();
    }

    public void TakeDamage(float damage, bool bCritical, Vector2 pos, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null)
    {

    }

    public void ApplyElementDebuff(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs, Vector2 pos = default)
    {
        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in debuffs)
        {
            if (currentAppliedDebuff.ContainsKey(pair.Key))
            {
                var data = currentAppliedDebuff[pair.Key];
                data.turnCnt += pair.Value.turnCnt;
                currentAppliedDebuff[pair.Key] = data;
            }
            else
            {
                currentAppliedDebuff[pair.Key] = pair.Value;
            }

        }

        PlayerDebuffChangedEvent?.Invoke();
    }

    public void ApplyElementDebuff(DebuffElementData debuff, Vector2 pos = default)
    {
        if (currentAppliedDebuff.ContainsKey(debuff.debuffElementType))
        {
            var data = currentAppliedDebuff[debuff.debuffElementType];
            data.turnCnt += debuff.turnCnt;
            currentAppliedDebuff[debuff.debuffElementType] = data;
        }
        else
        {
            currentAppliedDebuff[debuff.debuffElementType] = debuff;
        }

        PlayerDebuffChangedEvent?.Invoke();
    }

    public void ReleaseDebuff(DebuffElementData debuffElementData)
    {
        if (currentAppliedDebuff.ContainsKey(debuffElementData.debuffElementType))
        {
            var data = currentAppliedDebuff[debuffElementData.debuffElementType];
            data.turnCnt -= debuffElementData.turnCnt;
            currentAppliedDebuff[debuffElementData.debuffElementType] = data;
        }

        PlayerDebuffChangedEvent?.Invoke();
    }

    public void ReleaseDebuff(DebuffElementEffectType type)
    {
        currentAppliedDebuff.Remove(type);
        PlayerDebuffChangedEvent?.Invoke();
    }
}
