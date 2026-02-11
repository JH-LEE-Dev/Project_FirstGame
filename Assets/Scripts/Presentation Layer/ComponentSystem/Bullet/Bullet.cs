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
    protected IDamageSystem damageSystem;
    [SerializeField] public CircleCollider2D circleCollider;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask outOfRangeMask;

    //내부 의존성
    protected BulletStateMachine stateMachine;

    [SerializeField] private BulletBehavior behavior_BeforeFire_prefab;
    [SerializeField] private BulletBehavior behavior_Fly_prefab;
    [SerializeField] private BulletBehavior behavior_Hit_prefab;
    protected BulletBehavior behavior_BeforeFire;
    protected BulletBehavior behavior_Fly;
    protected BulletBehavior behavior_Hit;


    /// <summary>
    /// 구현 속성 존 --------------------------------------------------------
    /// </summary>

    protected bool bFired = false;
    public Vector2 initPosition { get; private set; }
    public Vector2 initDir { get; private set; }
    public float originRange { get; protected set; }
    public float originExplosionRange { get; protected set; }

    /// <summary>
    /// 시스템 코드 존 --------------------------------------------------------
    /// </summary>

    private void Awake()
    {

    }

    public virtual void Initialize(ICharacterStatProvider _characterStatProvider,
        IBulletEffectProvider _bulletEffectProvider,IDamageSystem _damageSystem)
    {
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        damageSystem = _damageSystem;

        stateMachine = GetComponent<BulletStateMachine>();

        ReadyBulletAttributes(_characterStatProvider, _bulletEffectProvider, _damageSystem);

        BulletBehaviorData data = new BulletBehaviorData();
        data.behavior_BeforeFire = behavior_BeforeFire;
        data.behavior_Fly = behavior_Fly;
        data.behavior_Hit = behavior_Hit;
        stateMachine.Initialize(characterStatProvider, bulletEffectProvider, damageSystem,this, data);
    }

    protected virtual void ReadyBulletAttributes(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        behavior_BeforeFire = Instantiate(behavior_BeforeFire_prefab);
        behavior_Fly = Instantiate(behavior_Fly_prefab);
        behavior_Hit = Instantiate(behavior_Hit_prefab);

        behavior_BeforeFire.Initialize(this, _characterStatProvider, _bulletEffectProvider, _damageSystem);
        behavior_Fly.Initialize(this,  _characterStatProvider, _bulletEffectProvider, _damageSystem);
        behavior_Hit.Initialize(this, _characterStatProvider, _bulletEffectProvider, _damageSystem);
    }

    private void OnDestroy()
    {
        BulletEffectIsFinishedEvent = null;
    }

    protected virtual void Update()
    {
        if (bFired)
        {
            stateMachine.Update();
        }
    }

    public virtual void Fire(Vector2 dir, Vector2 firePos) //발사하는 함수.
    {
        bFired = true;
        initPosition = firePos;
        initDir = dir;
        stateMachine.ChangeState<BS_BeforeFire>();
    }

    public void BulletEffectIsFinished() //총알의 공격 과정이 모두 끝났을 때 호출.
    {
        bFired = false;
        BulletEffectIsFinishedEvent?.Invoke();
    }











    /// <summary>
    /// 구현 코드 존 --------------------------------------------------------
    /// </summary>
}
