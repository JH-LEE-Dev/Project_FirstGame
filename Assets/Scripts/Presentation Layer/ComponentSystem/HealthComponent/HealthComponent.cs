using System;
using UnityEngine;

public class HealthComponent : EntityComponent, IStatusEffectReceiver
{
    public event Action UnitIsDeadEvent;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentShield;

    private float prevHealth;
    private float prevShield;

    bool bWeakness = false;

    protected override void Awake()
    {

    }

    public void ResetHealthComponent()
    {
        prevHealth = maxHealth;
        currentHealth = maxHealth;
        currentShield = 0;
        prevShield = 0;
    }

    public void ResetShield()
    {
        currentShield = 0;
        prevShield = 0;
    }

    protected override void OnDestroy()
    {
        UnitIsDeadEvent = null;
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

    public void SetWeakness(bool boolean)
    {
        bWeakness = boolean;
    }

    public void TakeDamange(float damage)
    {
        if(bWeakness)
        {
            damage += 1;
            Debug.Log("이 적은 현재 약화 상태입니다.");
        }

        prevHealth = currentHealth;
        prevShield = currentShield; 

        if (currentShield > 0)
        {
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

        if (currentHealth <= 0)
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

    public void IncreaseHP(float amount)
    {
        currentHealth += amount;
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
