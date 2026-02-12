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

    [SerializeField] private ArcDischarge arcDischarge_Prefab;
    private ArcDischarge arcDischarge;

    [SerializeField] private AquaBurst aquaBurst_Prefab;
    private AquaBurst aquaBurst;


    private Bullet currentBullet;


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

        PoolingBullets();
    }
    
    private void PoolingBullets()
    {
        prismBolt = Instantiate(prismBolt_Prefab);
        prismBolt.Initialize(characterStatProvider, bulletEffectProvider, damageSystem);
        prismBolt.BulletEffectIsFinishedEvent -= AttackFinished;
        prismBolt.BulletEffectIsFinishedEvent += AttackFinished;
        prismBolt.SetActive(false);


        arcDischarge = Instantiate(arcDischarge_Prefab);
        arcDischarge.Initialize(characterStatProvider, bulletEffectProvider, damageSystem);
        arcDischarge.BulletEffectIsFinishedEvent -= AttackFinished;
        arcDischarge.BulletEffectIsFinishedEvent += AttackFinished;
        arcDischarge.SetActive(false);


        aquaBurst = Instantiate(aquaBurst_Prefab);
        aquaBurst.Initialize(characterStatProvider, bulletEffectProvider, damageSystem);
        aquaBurst.BulletEffectIsFinishedEvent -= AttackFinished;
        aquaBurst.BulletEffectIsFinishedEvent += AttackFinished;
        aquaBurst.SetActive(false);

    }

    private void OnDestroy()
    {
        prismBolt.BulletEffectIsFinishedEvent -= AttackFinished;
        arcDischarge.BulletEffectIsFinishedEvent -= AttackFinished;
        aquaBurst.BulletEffectIsFinishedEvent -= AttackFinished;

        AttackFinishedEvent = null;
    }

    private void Update()
    {

    }

    public void Fire(BulletType _bulletType,int cnt, Vector2 dir, Vector2 firePos) //발사하는 함수.
    {
        switch (_bulletType)
        {
            case BulletType.PrismBolt:
                {
                    currentBullet = prismBolt;
                    prismBolt.SetActive(true);
                    prismBolt.Fire(dir, firePos);
                }
                break;
            case BulletType.ArcDischarge:
                {
                    currentBullet = arcDischarge;
                    arcDischarge.SetActive(true);
                    arcDischarge.Fire(dir, firePos);
                }
                break;
            case BulletType.AquaBurst:
                {
                    currentBullet = aquaBurst;
                    aquaBurst.SetActive(true);
                    aquaBurst.Fire(dir, firePos);
                }

                break;
        }
    }

    public void AttackFinished() //총알의 공격 과정이 모두 끝났을 때 호출.
    {
        currentBullet.SetActive(false);
        AttackFinishedEvent?.Invoke();
    }


    /// <summary>
    /// 구현 코드 존 --------------------------------------------------------
    /// </summary>
}
