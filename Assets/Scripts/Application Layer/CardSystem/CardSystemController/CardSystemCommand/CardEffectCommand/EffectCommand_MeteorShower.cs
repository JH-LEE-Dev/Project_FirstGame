using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/MeteorShower")]
public class EffectCommand_MeteorShower : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int attackCnt = 0;
    [SerializeField] private int bonusAttack = 0;
    [SerializeField] private int upgradedAttackCnt = 0;
    [SerializeField] private int upgradedBonusAttack = 0;
    [SerializeField] private int conditionCheckCardId = 0;
    [SerializeField] private int upgradedConditionCheckCardId = 0;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (nestingCnt != 0)
        {
            if (complexSystemActionCommandHandler.DeckConditionCheck(conditionCheckCardId))
            {
                complexSystemActionCommandHandler.ApplyAttackModifier(bonusAttack * nestingCnt * valueModifier);
                complexSystemActionCommandHandler.ApplyAttackCntModifier(attackCnt * nestingCnt * valueModifier);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            if (complexSystemActionCommandHandler.DeckConditionCheck(upgradedConditionCheckCardId))
            {
                complexSystemActionCommandHandler.ApplyAttackModifier(upgradedBonusAttack * upgradeNestingCnt * valueModifier);
                complexSystemActionCommandHandler.ApplyAttackCntModifier(upgradedAttackCnt * upgradeNestingCnt * valueModifier);
            }
        }

        ResetCommandData();
    }
}