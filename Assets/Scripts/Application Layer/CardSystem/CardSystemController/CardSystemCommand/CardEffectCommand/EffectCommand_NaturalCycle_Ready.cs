using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Natural Cycle Ready")]
public class EffectCommand_NaturalCycle_Ready : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        complexSystemActionCommandHandler = _complexSystemActionCommandHandler;

        complexSystemActionCommandHandler.ObserveElementExplosionEvent(TargetEventOccured);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        complexSystemActionCommandHandler.CancelObserveElementExplosionEvent(TargetEventOccured);
    }

    private void TargetEventOccured(ElementExplosionType _type)
    {
        complexSystemActionCommandHandler.CancelObserveElementExplosionEvent(TargetEventOccured);

        if(followUpEffectCommand != null)
        {
            complexSystemActionCommandHandler.ReserveCardEffect(followUpEffectCommand);
        }
    }
}