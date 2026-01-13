using System;
using UnityEngine;

public class HealthComponent : EntityComponent, IShieldEffectReceiver
{
    public event Action UnitIsDeadEvent;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentShield;

    private float prevHealth;
    private float prevShield;

    protected override void Awake()
    {

    }

    protected override void OnDestroy()
    {

    }

    protected override void FixedUpdate()
    {

    }

    protected override void Update()
    {

    }

    protected override void Start()
    {

    }

    public void SetHealth(float _health)
    {
        maxHealth = _health;
        prevHealth = _health;
        currentHealth = _health;
    }

    public void TakeDamange(float damage)
    {
        prevHealth = currentHealth;
        prevShield = currentShield; 

        if (currentShield > 0)
        {
            Debug.Log(currentShield);

            if (currentShield < damage)
            {
                damage -= currentShield;
                currentShield = 0;
            }
            else
            {
                currentShield -= damage;
                return;
            }
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            UnitIsDeadEvent?.Invoke();
            currentHealth = 0;
        }
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public float GetCurrentShield()
    {
        return currentShield;
    }

    public float GetPrevHealth()
    {
        return prevHealth;
    }
    public float GetPrevShield()
    {
        return prevShield;
    }


    public void ApplyShieldModifier(float bonusShield)
    {
        currentShield += bonusShield;
    }
}
