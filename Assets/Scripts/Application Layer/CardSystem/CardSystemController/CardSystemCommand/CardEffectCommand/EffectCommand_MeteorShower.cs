using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/MeteorShower")]
public class EffectCommand_MeteorShower : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int bonusAttack = 0;
    [SerializeField] private int upgradedBonusAttack = 0;

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        var handPile = handler.cardLogicSystem.GetHandPile();

        if (bUpgraded == false)
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(bonusAttack * valueModifier);
        }
        else
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(upgradedBonusAttack * valueModifier);
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        if (bUpgraded == false)
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(-bonusAttack * valueModifier);
        }
        else
        {
            handler.statusSystem.ApplyAdditionalAttackModifier(-upgradedBonusAttack * valueModifier);
        }
    }
}