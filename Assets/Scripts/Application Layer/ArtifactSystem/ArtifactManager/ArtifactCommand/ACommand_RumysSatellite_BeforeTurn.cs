using UnityEngine;

[CreateAssetMenu(menuName = "Command/ArtifactEffects/Rumy's Satellite BeforeTurn")]
public class ACommand_RumysSatellite_BeforeTurn : ArtifactCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        complexSystemActionCommand.ApplySlotCntModifier(2);
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        complexSystemActionCommand.ApplySlotCntModifier(-2);
    }
}