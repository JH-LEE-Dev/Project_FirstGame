using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/MeteorShower")]
public class EffectCommand_MeteorShower : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int attackCnt = 0;
    [SerializeField] private int upgradedAttackCnt = 0;
    [SerializeField] private int conditionCheckCardId = 0;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (complexSystemActionCommandHandler.DeckConditionCheck(conditionCheckCardId))
        {
            if (nestingCnt != 0)
                complexSystemActionCommandHandler.ApplyAttackCntModifier(attackCnt*nestingCnt*valueModifier);

            if(upgradeNestingCnt != 0)
                complexSystemActionCommandHandler.ApplyAttackCntModifier(upgradedAttackCnt * upgradeNestingCnt * valueModifier);
        }

        ResetCommandData();
    }
}