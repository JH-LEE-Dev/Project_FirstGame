using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    protected UnitContext ctx;
    protected bool bDead = false;

    public virtual void Initialize(UnitContext _ctx)
    {
        ctx = _ctx;
        BindEvent();
    }

    protected virtual void Awake()
    {

    }

    protected virtual void OnDestroy()
    {
        ReleaseEvent();
    }

    private void BindEvent()
    {
        ctx.unit.UnitIsDeadEvent -= UnitIsDead;
        ctx.unit.UnitIsDeadEvent += UnitIsDead;
    }

    private void ReleaseEvent()
    {
        if (ctx != null)
            ctx.unit.UnitIsDeadEvent -= UnitIsDead;
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void Start()
    {

    }

    private void UnitIsDead()
    {
        bDead = true;
    }
}
