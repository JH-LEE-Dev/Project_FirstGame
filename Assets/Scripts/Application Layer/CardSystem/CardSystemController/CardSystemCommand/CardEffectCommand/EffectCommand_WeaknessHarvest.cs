using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/WeaknessHarvest")]
public class EffectCommand_WeaknessHarvest : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private int enemyCnt = 0;

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        enemyCnt = 0;

        var enemyUnits = _handler.statusSystem.GetEnemyHandlers();

        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            if (enemyUnits[i].enemyData.currentAppliedDebuff.Count != 0 && enemyUnits[i].enemyData.bDead == false)
                ++enemyCnt;
        }

        if (bUpgraded == false)
        {
            _handler.statusSystem.ApplyAdditionalAttackModifier(enemyCnt * 2 * valueModifier);
            _handler.statusSystem.ApplyCriticalChanceModifier(enemyCnt * 2 * valueModifier);
            _handler.statusSystem.ApplyAttackRangeModifier(enemyCnt * 2 * valueModifier);
        }
        else
        {
            _handler.statusSystem.ApplyAdditionalAttackModifier(enemyCnt * 3 * valueModifier);
            _handler.statusSystem.ApplyCriticalChanceModifier(enemyCnt * 3 * valueModifier);
            _handler.statusSystem.ApplyAttackRangeModifier(enemyCnt * 3 * valueModifier);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler _handler)
    {
        if (bUpgraded == false)
        {
            _handler.statusSystem.ApplyAdditionalAttackModifier(-enemyCnt * 2 * valueModifier);
            _handler.statusSystem.ApplyCriticalChanceModifier(-enemyCnt * 2 * valueModifier);
            _handler.statusSystem.ApplyAttackRangeModifier(-enemyCnt * 2 * valueModifier);
        }
        else
        {
            _handler.statusSystem.ApplyAdditionalAttackModifier(-enemyCnt * 3 * valueModifier);
            _handler.statusSystem.ApplyCriticalChanceModifier(-enemyCnt * 3 * valueModifier);
            _handler.statusSystem.ApplyAttackRangeModifier(-enemyCnt * 3 * valueModifier);
        }
    }
}