using UnityEngine;
using System;
using Unity.VisualScripting;

public class Earth : MonoBehaviour, IDamageable, IPlayerData
{
    public event Action<float> TakeDamageEvent;
    public event Action PlayerDeadEvent;

    public IStatusEffectReceiver statusEffectReceiver => healthComponent;

    protected HealthComponent healthComponent;

    private int money = 0;

    private void Awake()
    {

    }

    public void Initialize()
    {
        healthComponent = GetComponent<HealthComponent>();

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

    public void TakeDamage(float damage, bool bCritical)
    {
        healthComponent.TakeDamage(damage);
        TakeDamageEvent?.Invoke(damage);
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
}
