using UnityEngine;

public class ECombatComponent : EntityComponent
{
    [SerializeField] private float attack = 0f;

    public void Initialize(float _attack)
    {
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
