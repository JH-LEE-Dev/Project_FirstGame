using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour, IDamageable
{
    [Header("Components")]
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sr;
    protected MoveComponent moveComponent;
    protected UnitContext ctx;
 
    public Animator animator { get; private set; }

    [Header("Command System")]
    protected readonly Queue<ICommand> commandQueue = new Queue<ICommand>();

    protected WaveManager waveManager;
    protected InputManager inputManager;

    protected Vector2 moveDirection;

    public virtual void Initialize(InputManager _inputManager,WaveManager _waveManager = null,
        EnemyTypeData _enemyTypeData= null)
    {
        inputManager = _inputManager;
        waveManager = _waveManager;
    }

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        moveComponent = GetComponent<MoveComponent>();

        ctx = new UnitContext();
        ctx.Initialize(this);

        moveComponent.Initialize(ctx);

        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.angularDamping = 1.5f;
    }

    protected virtual void OnDestroy()
    {
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
        moveDirection = move;

        moveComponent.SetMoveDirection(moveDirection);
    }

    public virtual void OnMove()
    {
    }

    public virtual void ApplyDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
}
