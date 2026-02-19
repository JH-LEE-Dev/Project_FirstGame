using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/ElementalBoost")]
public class EffectCommand_ElementalBoost : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private float bonusDamage = 0f;
    [SerializeField] private int bonusCrit = 0;
    [SerializeField] private float upgradedBonusDamage = 0f;
    [SerializeField] private int upgradedBonusCrit = 0;

    private bool bApplied = false;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        bApplied = false;

        var currentElement = complexSystemActionCommand.GetCurrentAppliedBulletElement();

        if (currentElement.Count != 0)
        {
            if (bUpgraded == false)
            {
                bApplied = true;
                complexSystemActionCommand.ApplyAdditionalAttackModifier(bonusDamage * valueModifier, GameSystemActionContextType.MAX);
                complexSystemActionCommand.ApplyCriticalChanceModifier(bonusCrit * valueModifier, GameSystemActionContextType.MAX);
            }
            else
            {
                bApplied = true;
                complexSystemActionCommand.ApplyAdditionalAttackModifier(upgradedBonusDamage * valueModifier, GameSystemActionContextType.MAX);
                complexSystemActionCommand.ApplyCriticalChanceModifier(upgradedBonusCrit * valueModifier, GameSystemActionContextType.MAX);
            }
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (bApplied == true)
        {
            if (bUpgraded == false)
            {
                complexSystemActionCommand.ApplyAdditionalAttackModifier(-bonusDamage * valueModifier, GameSystemActionContextType.MAX);
                complexSystemActionCommand.ApplyCriticalChanceModifier(-bonusCrit * valueModifier, GameSystemActionContextType.MAX);
            }
            else
            {
                complexSystemActionCommand.ApplyAdditionalAttackModifier(-upgradedBonusDamage * valueModifier, GameSystemActionContextType.MAX);
                complexSystemActionCommand.ApplyCriticalChanceModifier(-upgradedBonusCrit * valueModifier, GameSystemActionContextType.MAX);
            }
        }
    }
}
