using UnityEngine;

[CreateAssetMenu(menuName = "Strategy/BulletBehavior/Normal_BeforeFire")]
public class Normal_BeforeFire : BulletBehavior
{
    //필요하다면 사용.
    //public void Initialize(Bullet _bullet, ICharacterStatProvider _characterStatProvider, IBulletEffectProvider _bulletEffectProvider)
    //{
    //    base.Initialize(_bullet, _characterStatProvider, _bulletEffectProvider);
    //}

    public override void Enter()
    {
        bBehaviorEnd = false;
    }

    public override void Update()
    {
        if (bBehaviorEnd)
            return;

        End();
    }

    public override void End()
    {
        bBehaviorEnd = true;

        BulletBehaviorEndEvent?.Invoke();
    }
}