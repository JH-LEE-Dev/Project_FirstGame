using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/WeaknessHarvest")]
public class EffectCommand_WeaknessHarvest : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private int enemyCnt = 0;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        enemyCnt = 0;

        var enemyUnits = complexSystemActionCommandHandler.GetEnemyHandlers();

        for (int i = 0; i < enemyUnits.Count; ++i)
        {
            if (enemyUnits[i].currentAppliedDebuff.Count != 0 && enemyUnits[i].bDead == false)
                ++enemyCnt;
        }

        if (bUpgraded == false)
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(enemyCnt * 2 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyCriticalChanceModifier(enemyCnt * 2 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyAttackRangeModifier(enemyCnt * 2 * valueModifier);
        }
        else
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(enemyCnt * 3 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyCriticalChanceModifier(enemyCnt * 3 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyAttackRangeModifier(enemyCnt * 3 * valueModifier);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (bUpgraded == false)
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(-enemyCnt * 2 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyCriticalChanceModifier(-enemyCnt * 2 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyAttackRangeModifier(-enemyCnt * 2 * valueModifier);
        }
        else
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(-enemyCnt * 3 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyCriticalChanceModifier(-enemyCnt * 3 * valueModifier, GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.ApplyAttackRangeModifier(-enemyCnt * 3 * valueModifier);
        }
    }
}