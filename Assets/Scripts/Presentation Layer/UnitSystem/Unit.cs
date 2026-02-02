using System;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour, IDamageable
{
    /// <summary>
    /// 시스템 속성 존. ----------------------------------------
    /// </summary>
    public event Action<Unit> UnitIsDeadEvent;

    protected InputManager inputManager;
    protected GameServiceLocator gameServiceLocator;
    protected bool bDead = false;
    protected bool bCanAction = false;

    /// <summary>
    /// 구현 속성 존. -----------------------------------------
    /// </summary>
    [Header("Components")]
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sr;
    protected UnitContext ctx;
    protected EffectComponent effectComponent;
    protected HealthComponent healthComponent;

    public Animator animator { get; private set; }

    protected Vector2 moveDirection;


    /// <summary>
    /// 시스템 코드 존. --------------------------------------
    /// </summary>

    public virtual void Initialize(InputManager _inputManager, GameServiceLocator _gameServiceLocator)
    {
        gameServiceLocator = _gameServiceLocator;
        inputManager = _inputManager;
    }

    private void BindEvent()
    {
        healthComponent.UnitIsDeadEvent -= HandleDead;
        healthComponent.UnitIsDeadEvent += HandleDead;
    }

    private void ReleaseEvent()
    {
        healthComponent.UnitIsDeadEvent -= HandleDead;
    }

    protected virtual void HandleDead()
    {
        bDead = true;
    }

    protected virtual void OnDestroy()
    {
        ReleaseEvent();
        UnitIsDeadEvent = null;
    }

    protected void UnitIsDead()
    {
        bDead = true;
        UnitIsDeadEvent?.Invoke(this);
    }


    public virtual void SetbCanAction()
    {
        bCanAction = true;
    }

    public virtual void ResetbCanAction()
    {
        bCanAction = false;
    }

    public bool IsUnitDead()
    {
        return bDead;
    }



    /// <summary>
    /// 구현 코드 존.----------------------------------------------
    /// </summary>

    protected virtual void Awake()
    {
        ctx = new UnitContext();
        ctx.Initialize(this);

        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        effectComponent = GetComponent<EffectComponent>();
        healthComponent = GetComponent<HealthComponent>();


        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.angularDamping = 1.5f;

        BindEvent();
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void Update()
    {
    }

    //체력 깎이는 함수.
    public virtual void TakeDamage(float damage, bool bCritical)
    {
        healthComponent.TakeDamange(damage);
    }

    //RigidBody의 Damping 설정하는 함수. Character는 무관.
    public void ResetDamping()
    {
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }

    public virtual void KnockBack(Vector2 dir, float power)
    {
        return;
    }

    public virtual void ApplyWeakness(int turnCnt)
    {
        throw new NotImplementedException();
    }
}
