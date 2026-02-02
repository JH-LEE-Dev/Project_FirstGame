using System;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class PCombatComponent : CombatComponent
{
    /// <summary>
    /// 시스템 속성 존. -----------------------------------------
    /// </summary>

    public event Action BulletEffectIsFinishedEvent;

    //외부 의존성
    ICharacterStatProvider characterStatProvider;

    [SerializeField] private Bullet bulletPrefab;
    private Bullet bulletObject;

    /// <summary>
    /// 구현 속성 존. ---------------------------------------------
    /// </summary>











    /// <summary>
    /// 시스템 코드 존. ---------------------------------------------
    /// </summary>
    
    public void Initialize(UnitContext _ctx, ICombatSignalHandler _combatSignalHandler, ICharacterStatProvider _characterStatProvider)
    {
        base.Initialize(_ctx, _combatSignalHandler);

        characterStatProvider = _characterStatProvider;

        bulletObject = Instantiate(bulletPrefab, transform);
        bulletObject.gameObject.SetActive(false);
        bulletObject.Initialize(characterStatProvider);

        BindEvent();
    }


    protected override void Awake()
    {
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
        BulletEffectIsFinishedEvent = null;
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
