using System.Collections.Generic;
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

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var handPile = complexSystemActionCommandHandler.GetHandPile();

        if (nestingCnt != 0)
        {
            if (ConditionCheckOnly(handPile))
            {
                complexSystemActionCommandHandler.ApplyAttackModifier(bonusAttack * nestingCnt * valueModifier);
                complexSystemActionCommandHandler.ApplyAttackCntModifier(attackCnt * nestingCnt * valueModifier);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            if (ConditionCheck(handPile))
            {
                complexSystemActionCommandHandler.ApplyAttackModifier(upgradedBonusAttack * upgradeNestingCnt * valueModifier);
                complexSystemActionCommandHandler.ApplyAttackCntModifier(upgradedAttackCnt * upgradeNestingCnt * valueModifier);
            }
        }

        ResetCommandData();
    }

    private bool ConditionCheckOnly(IReadOnlyList<CardDataInstance> _cards)
    {
        for (int i = 0; i < 1; ++i)
        {
            if (_cards[i].GetCardData().id != (int)CardName.MeteorShower)
                return false;
        }

        return true;
    }

    private bool ConditionCheck(IReadOnlyList<CardDataInstance> _cards)
    {
        for (int i = 0; i < _cards.Count; ++i)
        {
            if (_cards[i].GetCardData().id != (int)CardName.MeteorShower)
                return false;
        }

        return true;
    }
}