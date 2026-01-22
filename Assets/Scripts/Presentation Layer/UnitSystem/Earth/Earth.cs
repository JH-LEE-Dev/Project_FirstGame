using UnityEngine;
using System;

public class Earth : MonoBehaviour, IDamageable, IPlayerData
{
    public event Action<float> TakeDamageEvent;

    public IStatusEffectReceiver statusEffectReceiver => healthComponent;

    protected HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    public void TakeDamage(float damage)
    {
        healthComponent.TakeDamange(damage);
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

    private void OnDestroy()
    {
        TakeDamageEvent = null;
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
}
