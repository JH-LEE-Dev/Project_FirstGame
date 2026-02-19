using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Natural Cycle Ready")]
public class EffectCommand_NaturalCycle_Ready : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    private CardEffectCommand naturalCycleExecuteCommand = null;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes, Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);

        if (naturalCycleExecuteCommand == null)
        {
            naturalCycleExecuteCommand = Instantiate(followUpEffectCommand);
        }
    }

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        naturalCycleExecuteCommand.ResetCommandData();

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

        if (followUpEffectCommand != null)
        {
            complexSystemActionCommandHandler.ReserveCardEffect(followUpEffectCommand);
        }
    }
}