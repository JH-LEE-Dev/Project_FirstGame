using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    protected UnitContext ctx;

    protected void Initialize(UnitContext _ctx)
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

    }

    private void ReleaseEvent()
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
}
