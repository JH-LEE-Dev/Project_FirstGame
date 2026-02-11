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

    //내부 의존성
    protected BulletStateMachine stateMachine;











    /// <summary>
    /// 구현 속성 존 --------------------------------------------------------
    /// </summary>

    protected bool bFired = false;
    public Vector2 flyDir { get; private set; }












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

        stateMachine.Initialize(characterStatProvider, bulletEffectProvider, damageSystem,this);
    }

    protected virtual void ReadyBulletAttributes(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {

    }

    private void OnDestroy()
    {
        BulletEffectIsFinishedEvent = null;
    }

    protected virtual void Update()
    {
        if (bFired)
            stateMachine.Update();
    }

    public virtual void Fire(Vector2 dir, Vector2 firePos) //발사하는 함수.
    {
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
