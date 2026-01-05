using System;
using UnityEngine;

public class HealthComponent : EntityComponent
{
    public event Action UnitIsDeadEvent;

    [SerializeField] private float health;

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
        health = _health;
    }

    public void DecreaseHealth(float damage)
    {
        health -= damage;

        if (health < 0)
        {
            UnitIsDeadEvent?.Invoke();
            health = 0;
        }
    }
}
