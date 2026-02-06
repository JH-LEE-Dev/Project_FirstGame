using UnityEngine;

public class ECombatComponent : CombatComponent
{
    //외부 의존성
    IEnemyStatProvider enemyStatProvider;

    public void Initialize(UnitContext _ctx,ICombatSignalHandler _signalHandler,IEnemyStatProvider _enemyStatProvider)
    {
        base.Initialize(_ctx, _signalHandler);
        enemyStatProvider = _enemyStatProvider;
    }

    public void ApplyAttack(Collider2D other)
    {
        IDamageable target = other.GetComponent<IDamageable>();

        if(target != null )
        {
            target.TakeDamage(enemyStatProvider.attack, false);
        }
    }
}
