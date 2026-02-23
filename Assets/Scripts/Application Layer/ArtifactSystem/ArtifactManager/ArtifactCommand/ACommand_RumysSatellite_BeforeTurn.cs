using UnityEngine;

[CreateAssetMenu(menuName = "Command/ArtifactEffects/Rumy's Satellite BeforeTurn")]
public class ACommand_RumysSatellite_BeforeTurn : ArtifactCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        handler.cardSlotSystem.ApplySlotCntModifier(2);
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        handler.cardSlotSystem.ApplySlotCntModifier(-2);
    }
}