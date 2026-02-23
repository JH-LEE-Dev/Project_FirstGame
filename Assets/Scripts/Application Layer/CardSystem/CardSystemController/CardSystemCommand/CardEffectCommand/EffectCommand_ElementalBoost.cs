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

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        bApplied = false;

        var currentElement = handler.statusSystem.GetCurrentAppliedBulletElement();

        if (currentElement.Count != 0)
        {
            if (bUpgraded == false)
            {
                bApplied = true;
                handler.statusSystem.ApplyAdditionalAttackModifier(bonusDamage * valueModifier);
                handler.statusSystem.ApplyCriticalChanceModifier(bonusCrit * valueModifier);
            }
            else
            {
                bApplied = true;
                handler.statusSystem.ApplyAdditionalAttackModifier(upgradedBonusDamage * valueModifier);
                handler.statusSystem.ApplyCriticalChanceModifier(upgradedBonusCrit * valueModifier);
            }
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        if (bApplied == true)
        {
            if (bUpgraded == false)
            {
                handler.statusSystem.ApplyAdditionalAttackModifier(-bonusDamage * valueModifier);
                handler.statusSystem.ApplyCriticalChanceModifier(-bonusCrit * valueModifier);
            }
            else
            {
                handler.statusSystem.ApplyAdditionalAttackModifier(-upgradedBonusDamage * valueModifier);
                handler.statusSystem.ApplyCriticalChanceModifier(-upgradedBonusCrit * valueModifier);
            }
        }
    }
}
