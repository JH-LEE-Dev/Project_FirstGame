using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/WaterFog")]
public class EffectCommnad_WaterFog : CardEffectCommand<IStatusEffectCommandHandler>
{
    private DebuffElementEffectType targetDebuff = DebuffElementEffectType.Wet;

    protected override void Execute(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {
        var enemies = cardStatusEffectCommandHandler.GetEnemyHandlers();

        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i].currentAppliedDebuff.ContainsKey(targetDebuff))
            {
                IEnemyHandler target = enemies[i];

                if (target != null)
                {
                    if (bUpgraded == false)
                    {
                        var targets = GetCollider(target,bUpgraded);

                        if (targets == null)
                            return;

                        for(int j = 0;j<targets.Count();++j)
                        {

                        }
                    }
                    else
                    {
                        var targets = GetCollider(target, bUpgraded);

                        if (targets == null)
                            return;

                        for (int j = 0; j < targets.Count(); ++j)
                        {

                        }
                    }
                }
            }
        }
    }

    private Collider2D[] GetCollider(IEnemyHandler _enemyHandler,bool _bUpgraded)
    {
        float radius = _enemyHandler.statusCollider.radius;
        if (_bUpgraded)
            radius *= 2;

        return Physics2D.OverlapCircleAll(
            _enemyHandler.GetTransform().position,
            radius,
           LayerMask.GetMask("Enemy"));
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}