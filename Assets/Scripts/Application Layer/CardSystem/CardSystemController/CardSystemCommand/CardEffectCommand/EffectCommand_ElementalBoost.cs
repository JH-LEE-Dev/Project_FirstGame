using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/ElementalBoost")]
public class EffectCommand_ElementalBoost : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private float bonusDamage = 0f;
    [SerializeField] private int bonusCrit = 0;
    [SerializeField] private float upgradedBonusDamage = 0f;
    [SerializeField] private int upgradedBonusCrit = 0;
    private IComplexSystemActionCommandHandler handler = null;
    private bool bApplied = false;

    public override bool EffectConditionCheck()
    {
        CalcValueModifier();

        if (handler == null)
            return false;

        var currentElement = handler.statusSystem.GetCurrentAppliedBulletElement();

        if (currentElement.Count == 0)
            return false;

        int newCondition = 0;

        if (newCondition != condition)
        {
            CheckApplyCondition();
            condition = newCondition;
        }

        return true;
    }

    private void CalcValueModifier()
    {
        if (cardEffectData.effectModifiers.ContainsKey(EffectModType.AllValueModifier))
        {
            valueModifier = cardEffectData.effectModifiers[EffectModType.AllValueModifier].value;
        }
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        handler = _handler;
        bApplied = false;

        if (EffectConditionCheck() == false)
            return;

        var currentElement = _handler.statusSystem.GetCurrentAppliedBulletElement();

        if (bUpgraded == false)
        {
            bApplied = true;
            _handler.statusSystem.ApplyAdditionalAttackModifier(bonusDamage * valueModifier);
            _handler.statusSystem.ApplyCriticalChanceModifier(bonusCrit * (int)valueModifier);
        }
        else
        {
            bApplied = true;
            _handler.statusSystem.ApplyAdditionalAttackModifier(upgradedBonusDamage * valueModifier);
            _handler.statusSystem.ApplyCriticalChanceModifier(upgradedBonusCrit * (int)valueModifier);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        if (bApplied == true)
        {
            if (bUpgraded == false)
            {
                handler.statusSystem.ApplyAdditionalAttackModifier(-bonusDamage * valueModifier);
                handler.statusSystem.ApplyCriticalChanceModifier(-bonusCrit * (int)valueModifier);
            }
            else
            {
                handler.statusSystem.ApplyAdditionalAttackModifier(-upgradedBonusDamage * valueModifier);
                handler.statusSystem.ApplyCriticalChanceModifier(-upgradedBonusCrit * (int)valueModifier);
            }
        }
    }
}
