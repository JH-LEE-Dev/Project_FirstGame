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
    public event Action UnitIsDeadEvent;
    [Header("Command System")]
    protected readonly Queue<ICommand> commandQueue = new Queue<ICommand>();

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
    protected MoveComponent moveComponent;
    protected UnitContext ctx;
    protected EffectComponent effectComponent;
    protected HealthComponent healthComponent;
    protected CombatComponent combatComponent;
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
        UnitIsDeadEvent = null;
    }

    public void HandleDead()
    {
        gameObject.SetActive(false);
    }


    public void ProcessNextCommand()
    {
        if (commandQueue == null)
        {
            Debug.Log("commandQueue is null -> Entity::ProcessNextCommand");
            return;
        }

        if (commandQueue.Count == 0)
            return;

        ICommand command = commandQueue.Dequeue();

        if (command == null)
        {
            Debug.Log("command is null -> Entity::ProcessNextCommand");
            return;
        }

        command.Execute(this);
    }

    public void EnqueueCommand(ICommand command)
    {
        if (command == null)
        {
            Debug.Log("command is null -> Entity::EnqueueCommand");
            return;
        }

        commandQueue.Enqueue(command);
    }

    protected virtual void OnDestroy()
    {
        ReleaseEvent();
    }

    protected void InvokeUnitIsDead()
    {
        UnitIsDeadEvent?.Invoke();
    }


    public virtual void SetbCanAction()
    {
        bCanAction = true;
    }

    public virtual void ResetbCanAction()
    {
        bCanAction = false;
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
        moveComponent = GetComponent<MoveComponent>();
        effectComponent = GetComponent<EffectComponent>();
        healthComponent = GetComponent<HealthComponent>();
        combatComponent = GetComponent<CombatComponent>();


        moveComponent.Initialize(ctx);

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
        //미구현. 신경쓰지 마쇼
        ProcessNextCommand();
    }

    //입력 시스템에 의해서 호출되는 움직임 함수.
    public virtual void OnMove(Vector2 move)
    {
        //시스템에 의해 플레이어가 공격 가능한 턴/타이밍에만 실행되게 적용.
        if (bCanAction == false)
        {
            return;
        }

        //키보드 <-, -> 에 따른 이동 방향임. Vector2(1,0) Vector2(-1,0)
        moveDirection = move;
        moveComponent.SetMoveDirection(moveDirection);
    }

    //이 함수는 입력에 의해 작동하지 않는 움직임에 필요한 함수.
    //ex. Enemy
    public virtual void OnMove()
    {

    }

    //체력 깎이는 함수.
    public virtual void TakeDamage(float damage)
    {
        healthComponent.DecreaseHealth(damage);
    }

    //RigidBody의 Damping 설정하는 함수. Character는 무관.
    public void ResetDamping()
    {
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }
}
