using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    protected UnitContext ctx;
    public virtual void Initialize(UnitContext _ctx)
    {
        ctx = _ctx;
    }

    protected virtual void Awake()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void Start()
    {

    }
}
