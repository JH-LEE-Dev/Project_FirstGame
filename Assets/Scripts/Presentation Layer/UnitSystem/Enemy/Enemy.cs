using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Unit, IEnemyData, IEnemyHandler
{
    //이벤트
    public event Action<IEnemyData, EnemyTypeData> EnemyIsKilledEvent;
    public event Action<IEnemyData, float, bool> EnemyTakeDamageEvent;
    public event Action EnemySpawnedEvent;
    public event Action EnemyIsDeadEvent;
    public event Action EnemyDebuffChangedEvent;
    public event Action<IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData>, DebuffElementData,Vector2> EnemyDebuffAppliedEvent;
    public event Action<IEnemyData, IEnemyData, Vector2> EnemyCollideEvent;
    public event Action<IEnemyData, IReadOnlyDictionary<BulletElementType, BulletElementData>, Vector2> EnemyHitEvent;

    //인터페이스 선언부.
    public IHealthComponentProvider healthComponentProvider => healthComponent;
    public IEnemyStatProvider enemyStatProvider => statComponent;
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> IEnemyData.currentAppliedDebuff => currentAppliedDebuff;
    bool IEnemyData.bDead => bDead;
    CircleCollider2D IEnemyHandler.statusCollider => statusCollider;
    public IEnemyData enemyData => this;

    //내부 의존성
    EVisualComponentCoordinator visualComponentCoordinator; //Visual 로직 통신을 담당하는 객체.
    ElementDamageHandleComponent elementDamageHandleComponent;




    /// <summary>
    /// 시스템 속성 존 .-----------------------------------
    /// </summary>

    [SerializeField] private LayerMask gravityLayerMask;
    public EnemyTypeData enemyTypeData { get; private set; }
    public int enemyID { get; private set; }

    [SerializeField] public CircleCollider2D statusCollider;


    private EMoveComponent moveComponent;
    private ECombatComponent combatComponent;
    private EStatComponent statComponent;
    bool bInitialized = false;
    private float activateDelay = 1f;
    private Vector2 targetPoint; //지구를 뜻함.
    bool bCanMove = false;



    /// <summary>
    /// 구현 속성 존. ------------------------------------
    /// </summary>

    private bool bAccelerate = false; // true -> 지구로 돌진할 때를 의미.
    private float initialDamping = 5f; // 원래 마찰력 저장용
    private float initialAngularDamping = 1.5f; // 원래 마찰력 저장용
    private int weaknessTurn = 0; // 현재 적용된 약화 턴 수.
    [SerializeField] private ParticleSystem vfxDeadImpact;








    /// <summary>
    /// 시스템 코드 존. -------------------------------------
    /// </summary>
    /// 
    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize_Enemy(InputManager _inputManager, GameServiceLocator _gameServiceLocator
     , EnemyTypeData _enemyTypeData)
    {
        if (bInitialized == false)
        {
            base.Initialize(_inputManager, _gameServiceLocator);

            combatComponent = GetComponent<ECombatComponent>();
            moveComponent = GetComponent<EMoveComponent>();
            visualComponentCoordinator = new EVisualComponentCoordinator();
            statComponent = GetComponent<EStatComponent>();
            elementDamageHandleComponent = new ElementDamageHandleComponent();

            //Visual 로직에 필요한 의존성을 추가해주면 됨.
            visualComponentCoordinator.Initialize(combatComponent, moveComponent);
            moveComponent.Initialize(ctx, visualComponentCoordinator);
            elementDamageHandleComponent.Initialize(currentAppliedDebuff);
            effectComponent.Initialize();
        }

        enemyTypeData = _enemyTypeData;
        SetupEnemyType();

        bInitialized = true;
    }

    public void SetbCanMove(bool boolean)
    {
        bCanMove = boolean;
    }

    private void SetEnemyState(bool boolean)
    {
        bAccelerate = false;

        if (moveComponent != null)
            moveComponent.SetAccelerate(bAccelerate);

        col.enabled = boolean;
        sr.enabled = boolean;

        bDead = !boolean;

        ClearDebuff();

        if (boolean == false)
        {
            rb.simulated = false;
        }
        else
        {
            rb.simulated = true;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public IEnumerator SetEnemyState_Delayed(bool boolean)
    {
        yield return new WaitForSeconds(activateDelay);

        if (boolean == true)
        {
            Physics2D.SyncTransforms();
        }

        SetEnemyState(boolean);
    }

    public void DeActivate()
    {
        SetEnemyState(false);
        EnemyIsDeadEvent?.Invoke();
        healthComponent.ResetHealthComponent();
    }

    public void Activate(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        healthComponent.ResetHealthComponent();

        sr.enabled = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        bAccelerate = false;
        if (moveComponent != null)
            moveComponent.SetAccelerate(bAccelerate);

        rb.linearDamping = initialDamping;
        rb.angularDamping = initialAngularDamping;

        SetEnemyState(true);
        //StartCoroutine(SetEnemyState_Delayed(true));

        EnemySpawnedEvent?.Invoke();
    }

    public void SetEnemyID(int _id)
    {
        enemyID = _id;
    }

    private void SetupEnemyType()
    {
        sr.sprite = enemyTypeData.sprite;
        float scale = enemyTypeData.scale;
        float scaleDelta = UnityEngine.Random.Range(0f, 1f);
        transform.localScale = new Vector3(scaleDelta + scale, scaleDelta + scale, 1f);
        moveComponent.SetImpulsePower(enemyTypeData.moveForce);
        healthComponent.SetHealth(enemyTypeData.health);

        statComponent.Initialize(enemyTypeData.attack);

        //비주얼 관련 초기화.
        combatComponent.Initialize(ctx, visualComponentCoordinator, statComponent);
    }

    public override void TakeDamage(float damage, bool bCritical, Vector2 pos, IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements = null)
    {
        if (bDead == true)
            return;

        damage = elementDamageHandleComponent.GetResultDamage(_bulletElements, damage);
        healthComponent.TakeDamage(damage);
        EnemyTakeDamageEvent?.Invoke(this, damage, bCritical);

        if (_bulletElements != null)
            EnemyHitEvent?.Invoke(this, _bulletElements, pos);
    }

    private void EnemyIsKilled()
    {
        EnemyIsKilledEvent?.Invoke(this, enemyTypeData);

        SetEnemyState(false);

        vfxDeadImpact.Play(true);
    }

    public void ResetState()
    {
        if (weaknessTurn > 0)
            weaknessTurn -= 1;

        if (weaknessTurn == 0)
            healthComponent.SetWeakness(false);
    }

    protected override void HandleDead()
    {
        base.HandleDead();

        EnemyIsKilled();
    }

    public override void ApplyWeakness(int turnCnt)
    {
        weaknessTurn = turnCnt;
        if (weaknessTurn > 0)
            healthComponent.SetWeakness(true);
    }

    //지구에 충돌했을 때 호출됨.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger || bDead || rb.simulated == false)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            EnemyCollideEvent?.Invoke(this, other as IEnemyData, hitPoint);

            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Earth"))
        {
            effectComponent.PlayExplosionEffect();

            gameServiceLocator.PlayCameraShake();

            combatComponent.ApplyAttack(other, currentAppliedDebuff);

            UnitIsDead();

            SetEnemyState(false);

            EnemyIsDeadEvent?.Invoke();

            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Gravity"))
        {
            bAccelerate = true;
            moveComponent.SetAccelerate(bAccelerate);
            ResetDamping();

            return;
        }
    }

    public override void KnockBack(Vector2 dir, float power)
    {
        moveComponent.ApplyKnockBack(dir, power);
    }

    public override void ApplyElementDebuff(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs, Vector2 pos = default)
    {
        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in debuffs)
        {
            EnemyDebuffAppliedEvent?.Invoke(currentAppliedDebuff, pair.Value, pos);

            if (currentAppliedDebuff.ContainsKey(pair.Key))
            {
                var data = currentAppliedDebuff[pair.Key];
                data.turnCnt += pair.Value.turnCnt;
                currentAppliedDebuff[pair.Key] = data;
            }
            else
            {
                currentAppliedDebuff[pair.Key] = pair.Value;
            }
        }

        EnemyDebuffChangedEvent?.Invoke();
    }

    public override void ApplyElementDebuff(DebuffElementData debuff, Vector2 pos = default)
    {
        EnemyDebuffAppliedEvent?.Invoke(currentAppliedDebuff, debuff, pos);

        if (currentAppliedDebuff.ContainsKey(debuff.debuffElementType))
        {
            var data = currentAppliedDebuff[debuff.debuffElementType];
            data.turnCnt += debuff.turnCnt;
            currentAppliedDebuff[debuff.debuffElementType] = data;
        }
        else
        {
            currentAppliedDebuff[debuff.debuffElementType] = debuff;
        }

        EnemyDebuffChangedEvent?.Invoke();
    }

    public void ClearDebuff()
    {
        if (bDead == false)
        {
            currentAppliedDebuff.Clear();
            EnemyDebuffChangedEvent?.Invoke();
        }
    }

    public void EnemyTurnEnd()
    {
        Span<DebuffElementEffectType> allKeys = stackalloc DebuffElementEffectType[currentAppliedDebuff.Count];
        int index = 0;

        foreach (var k in currentAppliedDebuff.Keys)
            allKeys[index++] = k;

        for (int i = 0; i < allKeys.Length; i++)
        {
            var key = allKeys[i];
            var data = currentAppliedDebuff[key];

            if (data.turnCnt <= 1)
            {
                currentAppliedDebuff.Remove(key);
            }
            else
            {
                data.turnCnt -= 1;
                currentAppliedDebuff[key] = data;
            }
        }

        EnemyDebuffChangedEvent?.Invoke();
    }

    public void ReleaseDebuff(DebuffElementData debuffElementData)
    {
        if (currentAppliedDebuff.ContainsKey(debuffElementData.debuffElementType))
        {
            var data = currentAppliedDebuff[debuffElementData.debuffElementType];
            data.turnCnt -= debuffElementData.turnCnt;
            currentAppliedDebuff[debuffElementData.debuffElementType] = data;
        }

        EnemyDebuffChangedEvent?.Invoke();
    }

    public void ReleaseDebuff(DebuffElementEffectType type)
    {
        currentAppliedDebuff.Remove(type);
        EnemyDebuffChangedEvent?.Invoke();
    }







    /// <summary>
    /// 구현 코드 존. ------------------------------------------
    /// </summary>

    //Enemy Turn이 시작되면 상위 모듈에서 호출해줌.
    public void OnMove()
    {
        if (bDead == false)
        {
            if (bCanMove == true)
                moveComponent.ApplyImpulse();
            else
                bCanMove = true;
        }
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void OnDestroy()
    {

    }

    public void SetTargetPoint(Vector2 _targetPoint)
    {
        targetPoint = _targetPoint;
        Vector2 targetDir = targetPoint - (Vector2)transform.position;
        targetDir.Normalize();

        moveComponent.SetMoveDirection(targetDir);
    }

    public float GetMaxHealth()
    {
        return healthComponent.GetMaxHealth();
    }

    public float GetCurrentHealth()
    {
        return healthComponent.GetCurrentHealth();
    }
}