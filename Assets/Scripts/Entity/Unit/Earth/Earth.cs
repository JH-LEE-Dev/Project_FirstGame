using UnityEngine;

public class Earth : MonoBehaviour, IDamageable, IEarthData
{
    public IShieldEffectReceiver shieldEffectReceiver => healthComponent;

    protected HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    public void TakeDamage(float damage)
    {
        healthComponent.DecreaseHealth(damage);
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
}
