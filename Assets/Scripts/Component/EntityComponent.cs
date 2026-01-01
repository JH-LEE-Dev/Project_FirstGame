using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    protected UnitContext ctx;
    protected bool bDead = false;

    public virtual void Initialize(UnitContext _ctx)
    {
        ctx = _ctx;
        ctx.unit.RegisterDeadListener(UnitIsDead);
    }

    protected virtual void Awake()
    {

    }

    protected virtual void OnDestroy()
    {

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
