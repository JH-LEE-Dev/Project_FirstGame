using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/Natural Cycle Ready")]
public class EffectCommand_NaturalCycle_Ready : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    IComplexSystemActionCommandHandler handler;

    private bool bExecuted = false;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes, Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        bExecuted = false;

        followUpEffectCommand.ResetCommandData();

        handler = _handler;

        handler.statusSystem.ElementExplosionOccuredEvent -= TargetEventOccured;
        handler.statusSystem.ElementExplosionOccuredEvent += TargetEventOccured;
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        handler.statusSystem.ElementExplosionOccuredEvent -= TargetEventOccured;
    }

    private void TargetEventOccured(ElementExplosionType _type)
    {
        if (bExecuted == true)
            return;

        handler.statusSystem.ElementExplosionOccuredEvent -= TargetEventOccured;

        if (followUpEffectCommand != null)
        {
            handler.cardSystem.ReserveCardEffect(followUpEffectCommand);
        }

        bExecuted = true;
    }
}