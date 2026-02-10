using UnityEngine;

[CreateAssetMenu(menuName = "Command/ArtifactEffects/Rumy's Satellite AfterCardUsing")]
public class ACommand_RumysSatellite_AfterCardUsing : ArtifactCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (complexSystemActionCommand.IsInherenceCardEquipped() == true)
            return;

        Debug.Log("루미의 위성 효과 적용.");

        AdditionalAttackStat additionalAttackStat = new AdditionalAttackStat(2, 0.2f, 1);

        if (bUpgraded == false)
        {
            complexSystemActionCommand.SetBulletType(BulletType.PrismBolt, false, additionalAttackStat);

            complexSystemActionCommand.ApplyAttackModifier(10,GameSystemActionContextType.MAX);
            complexSystemActionCommand.ApplyAdditionalAttackValueModifier(1);
        }
        else
        {
            complexSystemActionCommand.SetBulletType(BulletType.PrismBolt, false, additionalAttackStat);

            complexSystemActionCommand.ApplyAttackModifier(20, GameSystemActionContextType.MAX);
            complexSystemActionCommand.ApplyAdditionalAttackValueModifier(2);
        }

        complexSystemActionCommand.SetCharacterCanAttackState(true);
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        if (complexSystemActionCommand.IsInherenceCardEquipped() == true)
            return;

        complexSystemActionCommand.ResetBulletType();

        complexSystemActionCommand.SetCharacterCanAttackState(false);
    }
}