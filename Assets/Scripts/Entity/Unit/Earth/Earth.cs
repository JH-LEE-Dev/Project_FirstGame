using UnityEngine;
using System;

public class Earth : MonoBehaviour, IDamageable, IEarthData, IUnitEvent
{
    public event Action<float> TakeDamageEvent;

    public IShieldEffectReceiver shieldEffectReceiver => healthComponent;

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
}
