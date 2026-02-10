using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 시스템 속성 존 ----------------------------------------------
    /// </summary>

    //이벤트
    public event Action BulletEffectIsFinishedEvent;

    //외부 의존성
    ICharacterStatProvider characterStatProvider; //속성,스탯을 가져오는 컴포넌트
    IBulletEffectProvider bulletEffectProvider; //총알 타입을 가져오는 컴포넌트

    //내부 의존성
    public EffectComponent effectComponent {  get; private set; }
    public SpriteRenderer sr { get; private set; }
    private BulletStateMachine stateMachine;

    [SerializeField] public CircleCollider2D circleCollider;
    [SerializeField] public CircleCollider2D explosionRangeCollider;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask outOfRangeMask;

    private DamageCalcComponent damageCalcComponent;

    public float range { get; private set; }


    /// <summary>
    /// 구현 속성 존 --------------------------------------------------------
    /// </summary>

    public Vector2 flyDir { get; private set; }
    public Vector2 prevPosition { get; private set; }


    /// <summary>
    /// 시스템 코드 존 --------------------------------------------------------
    /// </summary>

    private void Awake()
    {

    }

    public void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider,
        DamageCalcComponent _damageCalculator)
    {
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        damageCalcComponent = _damageCalculator;

        sr = GetComponentInChildren<SpriteRenderer>();
        effectComponent = GetComponentInChildren<EffectComponent>();
        stateMachine = GetComponent<BulletStateMachine>();

        stateMachine.Initialize(characterStatProvider,bulletEffectProvider,this,damageCalcComponent);

        circleCollider.enabled = false;
        explosionRangeCollider.enabled = false;
        range = explosionRangeCollider.radius;
    }

    private void OnDestroy()
    {
        BulletEffectIsFinishedEvent = null;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void Fire(Vector2 dir) //발사하는 함수.
    {
        ActivateBullet();

        dir.Normalize();
        flyDir = dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        prevPosition = transform.position;

        stateMachine.ChangeState<BS_BeforeFire>();
    }

    public void BulletEffectIsFinished() //총알의 공격 과정이 모두 끝났을 때 호출.
    {
        DeActivateBullet();
        BulletEffectIsFinishedEvent?.Invoke();
    }

    private void DeActivateBullet()
    {
        sr.gameObject.SetActive(false);
        effectComponent.gameObject.SetActive(false);

        circleCollider.enabled = false;
        explosionRangeCollider.enabled = false;
    }

    private void ActivateBullet()
    {
        sr.gameObject.SetActive(true);
        effectComponent.gameObject.SetActive(true);

        circleCollider.enabled = true;
        explosionRangeCollider.enabled = true;
    }





    /// <summary>
    /// 구현 코드 존 --------------------------------------------------------
    /// </summary>
}
