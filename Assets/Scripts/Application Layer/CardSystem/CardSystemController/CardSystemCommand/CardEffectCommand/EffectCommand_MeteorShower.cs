using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/MeteorShower")]
public class EffectCommand_MeteorShower : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int bonusAttack = 0;
    [SerializeField] private int upgradedBonusAttack = 0;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var handPile = complexSystemActionCommandHandler.GetHandPile();

        if (nestingCnt != 0)
        {
            complexSystemActionCommandHandler.ApplyAttackModifier(bonusAttack * nestingCnt * valueModifier,cardSystemContextType);
        }

        if (upgradeNestingCnt != 0)
        {
            complexSystemActionCommandHandler.ApplyAttackModifier(upgradedBonusAttack * upgradeNestingCnt * valueModifier, cardSystemContextType);
        }

        ResetCommandData();
    }
}