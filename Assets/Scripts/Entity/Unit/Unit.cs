using System;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour, IDamageable
{
    public event Action UnitIsDeadEvent;

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

    [Header("Command System")]
    protected readonly Queue<ICommand> commandQueue = new Queue<ICommand>();

    protected InputManager inputManager;
    protected GameServiceLocator gameServiceLocator;

    protected Vector2 moveDirection;
    protected bool bDead = false;

    public virtual void Initialize(InputManager _inputManager, GameServiceLocator _gameServiceLocator)
    {
        gameServiceLocator = _gameServiceLocator;
        inputManager = _inputManager;
    }

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        moveComponent = GetComponent<MoveComponent>();
        effectComponent = GetComponent<EffectComponent>();
        healthComponent = GetComponent<HealthComponent>();
        combatComponent = GetComponent<CombatComponent>();

        ctx = new UnitContext();
        ctx.Initialize(this);

        moveComponent.Initialize(ctx);

        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.angularDamping = 1.5f;

        healthComponent.UnitIsDeadEvent -= HandleDead;
        healthComponent.UnitIsDeadEvent += HandleDead;
    }

    protected virtual void OnDestroy()
    {
        UnitIsDeadEvent = null;
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnEnable()
    {

    }
    protected virtual void Update()
    {
        ProcessNextCommand();
    }

    protected virtual void PollInit()
    {

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

    public virtual void OnMove(Vector2 move)
    {
        if (gameServiceLocator.IsGameState<GS_PlayerTurnState>() == false)
            return;

        moveDirection = move;

        moveComponent.SetMoveDirection(moveDirection);
    }

    public virtual void OnMove()
    {

    }

    public virtual void TakeDamage(float damage)
    {
        healthComponent.DecreaseHealth(damage);
    }

    public void ResetDamping()
    {
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }

    protected void InvokeUnitIsDead()
    {
        UnitIsDeadEvent?.Invoke();
    }

    public void RegisterDeadListener(Action listener)
    {
        UnitIsDeadEvent += listener;
    }

    public void HandleDead()
    {
        gameObject.SetActive(false);
    }
}
