using System;
using System.Collections;
using UnityEngine;
using WaveSystemSignals;


public class Enemy : Unit, IEnemyData
{
    public event Action<IEnemyData, EnemyTypeData> EnemyIsKilledEvent;
    public event Action<IEnemyData, float, bool> EnemyTakeDamageEvent;

    //내부 의존성
    EVisualComponentCoordinator visualComponentCoordinator; //Visual 로직 통신을 담당하는 객체.

    /// <summary>
    /// 시스템 속성 존 .-----------------------------------
    /// </summary>
    [SerializeField] private LayerMask gravityLayerMask;
    private EnemyTypeData enemyTypeData;
    private TrailRenderer trailRenderer; //임시 트레일임, 버려도 무방.
    private EMoveComponent moveComponent;
    private ECombatComponent combatComponent;
    private int weaknessTurn = 0;

    /// <summary>
    /// 구현 속성 존. ------------------------------------
    /// </summary>
    private Vector2 targetPoint; //지구를 뜻함.
    private bool bAccelerate = false; // true -> 지구로 돌진할 때를 의미.
    [SerializeField] private ParticleSystem vfxDeadImpact;

    bool bInitialized = false;

    private float activateDelay = 1f;

    private float initialDamping = 5f; // 원래 마찰력 저장용
    private float initialAngularDamping = 1.5f; // 원래 마찰력 저장용

    /// <summary>
    /// 시스템 코드 존. -------------------------------------
    /// </summary>
    /// 
    protected override void Awake()
    {
        base.Awake();
    }

    private void SetEnemyState(bool boolean)
    {
        bAccelerate = false;

        if (moveComponent != null)
            moveComponent.SetAccelerate(bAccelerate);

        col.enabled = boolean;
        sr.enabled = boolean;

        bDead = !boolean;

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

    public void ActivateEnemy()
    {
        SetEnemyState(true);
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

    public void DeActivateEnemy()
    {
        SetEnemyState(false);

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

        StartCoroutine(SetEnemyState_Delayed(true));
    }

    public void Initialize_Enemy(InputManager _inputManager, GameServiceLocator _gameServiceLocator
        , EnemyTypeData _enemyTypeData)
    {
        if (bInitialized == false)
        {
            base.Initialize(_inputManager, _gameServiceLocator);
            enemyTypeData = _enemyTypeData;

            combatComponent = GetComponent<ECombatComponent>();
            moveComponent = GetComponent<EMoveComponent>();
            visualComponentCoordinator = new EVisualComponentCoordinator();

            //Visual 로직에 필요한 의존성을 추가해주면 됨.
            visualComponentCoordinator.Initialize(combatComponent, moveComponent);
            moveComponent.Initialize(ctx, visualComponentCoordinator);

            //trail 임시 코드.
            trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.material = sr.material;
            trailRenderer.material.mainTexture = sr.sprite.texture;
            Color c = trailRenderer.material.color;
            c.a = 0.3f;
            trailRenderer.material.color = c;
        }

        SetupEnemyType();

        bInitialized = true;
    }

    private void SetupEnemyType()
    {
        sr.sprite = enemyTypeData.sprite;
        float scale = enemyTypeData.scale;
        float scaleDelta = UnityEngine.Random.Range(0f, 1f);
        transform.localScale = new Vector3(scaleDelta + scale, scaleDelta + scale, 1f);
        moveComponent.SetImpulsePower(enemyTypeData.moveForce);
        healthComponent.SetHealth(enemyTypeData.health);

        //비주얼 관련 초기화.
        combatComponent.Initialize(ctx, visualComponentCoordinator, enemyTypeData.attack);
    }

    public override void TakeDamage(float damage, bool bCritical)
    {
        healthComponent.TakeDamange(damage);
        EnemyTakeDamageEvent?.Invoke(this, damage, bCritical);
    }

    private void EnemyIsKilled()
    {
        EnemyIsKilledEvent?.Invoke(this, enemyTypeData);

        SetEnemyState(false);

        vfxDeadImpact.Play(true);
    }

    //Enemy Turn이 시작되면 상위 모듈에서 호출해줌.
    public void OnMove()
    {
        if (bDead == false)
        {
            moveComponent.ApplyImpulse();
        }
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


    /// <summary>
    /// 구현 코드 존. ------------------------------------------
    /// </summary>

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

    //지구에 충돌했을 때 호출됨.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger || bDead || rb.simulated == false)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Earth"))
        {
            effectComponent.PlayExplosionEffect();

            SetEnemyState(false);

            UnitIsDead();

            gameServiceLocator.PlayCameraShake();

            combatComponent.ApplyAttack(other);

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

    public Transform GetTransform()
    {
        return transform;
    }

    public float GetMaxHealth()
    {
        return healthComponent.GetMaxHealth();
    }

    public float GetCurrentHealth()
    {
        return healthComponent.GetCurrentHealth();
    }

    public override void KnockBack(Vector2 dir, float power)
    {
        moveComponent.ApplyKnockBack(dir, power);
    }
}