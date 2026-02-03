using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/ComplexSystemAction/HandPileExistEffectExecute")]
public class ActionCommand_HandPileExistEffectExecute : CardSystemActionCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        var handPile = complexSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == 0)
            return;

        using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for(int i = 0;i< handPile.Count;++i)
        {
            writeBuffer[i] = handPile[i];
        }

        complexSystemActionCommandHandler.ExecuteHandPileExistEffect(writeBuffer, cardSystemContextType);
    }
}
