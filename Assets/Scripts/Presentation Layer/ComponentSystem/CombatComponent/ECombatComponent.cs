using UnityEngine;

public class ECombatComponent : CombatComponent
{
    [SerializeField] private float attack = 0f;

    public void Initialize(UnitContext _ctx,ICombatSignalHandler _signalHandler,float _attack)
    {
        base.Initialize(_ctx, _signalHandler); 
        attack = _attack;
    }

    public void ApplyAttack(Collider2D other)
    {
        IDamageable target = other.GetComponent<IDamageable>();

        if(target != null )
        {
            target.TakeDamage(attack);
        }
    }
}
