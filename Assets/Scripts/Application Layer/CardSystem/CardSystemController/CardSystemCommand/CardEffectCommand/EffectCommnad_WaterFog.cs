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
                IDamageable target = (IDamageable)enemies[i];

                if(target != null)
                {
                    if (bUpgraded == false)
                        target.ApplyElementDebuff(targetDebuff, 1);
                    else
                        target.ApplyElementDebuff(targetDebuff, 2);
                }
            }
        }
    }

    protected override void Undo(IStatusEffectCommandHandler cardStatusEffectCommandHandler)
    {

    }
}