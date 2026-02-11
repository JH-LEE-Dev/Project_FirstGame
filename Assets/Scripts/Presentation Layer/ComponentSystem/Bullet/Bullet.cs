using System;
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
    private BulletStateMachine stateMachine;
    private DamageCalcComponent damageCalcComponent;

    [SerializeField] private Projectile projectileObj_prefab;
    [SerializeField] private NonProjectile nonProjectileObj_prefab;
    public Projectile projectileObj;
    public NonProjectile nonProjectileObj;


    /// <summary>
    /// 구현 속성 존 --------------------------------------------------------
    /// </summary>

    private bool bFired = false;
    public Vector2 flyDir {  get; private set; }

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

        stateMachine = GetComponent<BulletStateMachine>();

        projectileObj = Instantiate(projectileObj_prefab);
        nonProjectileObj = Instantiate(nonProjectileObj_prefab);

        stateMachine.Initialize(characterStatProvider, bulletEffectProvider, this, damageCalcComponent);
    }

    private void OnDestroy()
    {
        BulletEffectIsFinishedEvent = null;
    }

    private void Update()
    {
        if (bFired)
            stateMachine.Update();
    }

    public void Fire(Vector2 dir,Vector2 firePos) //발사하는 함수.
    {
        ActivateBullet();

        bFired = true;

        dir.Normalize();
        flyDir = dir;

        projectileObj.transform.position = firePos;
        nonProjectileObj.transform.position = firePos;

        stateMachine.ChangeState<BS_BeforeFire>();
    }

    public void BulletEffectIsFinished() //총알의 공격 과정이 모두 끝났을 때 호출.
    {
        bFired = false;
        DeActivateBullet();
        BulletEffectIsFinishedEvent?.Invoke();
    }

    private void DeActivateBullet()
    {

    }

    private void ActivateBullet()
    {

    }





    /// <summary>
    /// 구현 코드 존 --------------------------------------------------------
    /// </summary>
}
