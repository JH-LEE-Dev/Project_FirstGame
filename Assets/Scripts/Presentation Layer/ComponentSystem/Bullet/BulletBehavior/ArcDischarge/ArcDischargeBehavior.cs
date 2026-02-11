using UnityEngine;

public class ArcDischargeBehavior : BulletBehavior
{
    static protected Collider2D firstTarget;
    protected ArcDischarge arcDischarge;

    public override void Initialize(Bullet owner, ICharacterStatProvider _characterStatProvider,
                                    IBulletEffectProvider _bulletEffectProvider, IDamageSystem _damageSystem)
    {
        base.Initialize(owner, _characterStatProvider, _bulletEffectProvider, _damageSystem);
        arcDischarge = owner as ArcDischarge;
    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void End()
    {

    }

    public override void Exit()
    {

    }
}
