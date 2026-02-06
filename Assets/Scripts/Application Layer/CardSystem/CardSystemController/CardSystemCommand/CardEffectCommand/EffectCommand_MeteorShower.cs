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

        if (bUpgraded == false)
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(bonusAttack * valueModifier,gameSystemActionContext);
        }
        else
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(upgradedBonusAttack * valueModifier, gameSystemActionContext);
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (bUpgraded == false)
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(-bonusAttack * valueModifier, gameSystemActionContext);
        }
        else
        {
            complexSystemActionCommandHandler.ApplyAdditionalAttackModifier(-upgradedBonusAttack * valueModifier, gameSystemActionContext);
        }
    }
}