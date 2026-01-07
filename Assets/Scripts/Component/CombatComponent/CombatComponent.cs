using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatComponent : EntityComponent, ICombatEffectReceiver
{
    [SerializeField] private Bullet bulletPrefab;
    private Bullet bulletObject;

    public event Action BulletEffectIsFinishedEvent;

    protected override void Awake()
    {
        bulletObject = Instantiate(bulletPrefab,transform);
        bulletObject.gameObject.SetActive(false);

        bulletObject.BulletEffectIsFinishedEvent -= BulletEffectIsFinished;
        bulletObject.BulletEffectIsFinishedEvent += BulletEffectIsFinished;
    }

    protected override void OnDestroy()
    {
        bulletObject.BulletEffectIsFinishedEvent -= BulletEffectIsFinished;
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

    public virtual void Fire(Vector2 dir)
    {
        bulletObject.transform.position = transform.position;
        bulletObject.gameObject.SetActive(true);
        bulletObject.Fire(dir);
    }

    public void BulletEffectIsFinished()
    {
        BulletEffectIsFinishedEvent?.Invoke();
    }

    public void ApplyAttackModifier(float bonusDamage)
    {
        Debug.Log("ApplyAttack!");
    }
}
