using System;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    /// <summary>
    /// 시스템 속성 존 ----------------------------------------------
    /// </summary>

    //이벤트
    public event Action AttackFinishedEvent;

    //외부 의존성
    ICharacterStatProvider characterStatProvider; //속성,스탯을 가져오는 컴포넌트
    IBulletEffectProvider bulletEffectProvider; //총알 타입을 가져오는 컴포넌트
    private IDamageSystem damageSystem;


    /// <summary>
    /// 구현 속성 존 --------------------------------------------------------
    /// </summary>

    //내부 의존성
    [SerializeField] private PrismBolt prismBolt_Prefab;
    private PrismBolt prismBolt;







    /// <summary>
    /// 시스템 코드 존 --------------------------------------------------------
    /// </summary>

    private void Awake()
    {

    }

    public void Initialize(ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider,
        IDamageSystem _damageSystem)
    {
        characterStatProvider = _characterStatProvider;
        bulletEffectProvider = _bulletEffectProvider;
        damageSystem = _damageSystem;

        prismBolt = Instantiate(prismBolt_Prefab);
        prismBolt.Initialize(_characterStatProvider,_bulletEffectProvider,damageSystem);
    }

    private void OnDestroy()
    {
        AttackFinishedEvent = null;
    }

    private void Update()
    {

    }

    public void Fire(BulletType _bulletType,int cnt, Vector2 dir, Vector2 firePos) //발사하는 함수.
    {
        
    }

    public void AttackFinished() //총알의 공격 과정이 모두 끝났을 때 호출.
    {
        AttackFinishedEvent?.Invoke();
    }





    /// <summary>
    /// 구현 코드 존 --------------------------------------------------------
    /// </summary>
}
