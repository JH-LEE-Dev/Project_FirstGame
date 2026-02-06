using UnityEngine;

[CreateAssetMenu(menuName = "Command/ArtifactEffects/Rumy's Satellite")]
public class ACommand_RumysSatellite : ArtifactCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private float value = 0;
    [SerializeField] private float attackValue = 0;
    [SerializeField] private float upgradedvalue = 0;
    [SerializeField] private float upgradedAttackValue = 0;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (complexSystemActionCommand.IsInherenceCardEquipped() == true)
            return;

        Debug.Log("루미의 위성 효과 적용.");

        if (bUpgraded == false)
        {
            complexSystemActionCommand.ApplyAttackModifier(attackValue,GameSystemActionContextType.MAX);
            complexSystemActionCommand.ApplyTotalDamageModifier(value);
        }
        else
        {
            complexSystemActionCommand.ApplyAttackModifier(upgradedAttackValue, GameSystemActionContextType.MAX);
            complexSystemActionCommand.ApplyTotalDamageModifier(upgradedvalue);
        }

        complexSystemActionCommand.SetCharacterCanAttackState(true);
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (complexSystemActionCommand.IsInherenceCardEquipped() == true)
            return;

        if (bUpgraded == false)
        {
            complexSystemActionCommand.ApplyAttackModifier(-attackValue, GameSystemActionContextType.MAX);
            complexSystemActionCommand.UndoTotalDamageModifier(value);
        }
        else
        {
            complexSystemActionCommand.ApplyAttackModifier(-upgradedAttackValue, GameSystemActionContextType.MAX);
            complexSystemActionCommand.UndoTotalDamageModifier(upgradedvalue);
        }

        complexSystemActionCommand.SetCharacterCanAttackState(true);
    }
}