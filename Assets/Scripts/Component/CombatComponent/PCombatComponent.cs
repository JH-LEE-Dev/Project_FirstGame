using System;
using System.Collections.Generic;
using UnityEngine;

public class PCombatComponent : EntityComponent, ICombatEffectReceiver
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    public event Action BulletEffectIsFinishedEvent;

    [SerializeField] private Bullet bulletPrefab;
    private Bullet bulletObject;


    /// <summary>
    /// 구현 속성 존. ---------------------------------------------
    /// </summary>











    /// <summary>
    /// 시스템 코드 존. ---------------------------------------------
    /// </summary>


    protected override void Awake()
    {
        //총알 오브젝트 생성.
        bulletObject = Instantiate(bulletPrefab,transform);
        bulletObject.gameObject.SetActive(false);

        BindEvent();
    }

    private void BindEvent()
    {
        bulletObject.BulletEffectIsFinishedEvent -= BulletEffectIsFinished;
        bulletObject.BulletEffectIsFinishedEvent += BulletEffectIsFinished;
    }

    private void ReleaseEvent()
    {
        bulletObject.BulletEffectIsFinishedEvent -= BulletEffectIsFinished;
    }

    protected override void OnDestroy()
    {
        ReleaseEvent();
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
        bulletObject.ApplyAttackModifier(bonusDamage);
    }

    public bool CanApplyBulletEffect()
    {
        return bulletObject.CanApplyBulletEffect();
    }

    /// <summary>
    /// 구현 코드 존. ----------------------------------------------------
    /// </summary>

    protected override void FixedUpdate()
    {

    }

    protected override void Update()
    {

    }

    protected override void Start()
    {

    }
}
