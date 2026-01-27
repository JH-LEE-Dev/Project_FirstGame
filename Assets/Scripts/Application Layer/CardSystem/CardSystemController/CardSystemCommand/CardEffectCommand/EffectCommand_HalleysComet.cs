using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Halley's Comet")]
public class EffectCommand_HalleysComet : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        IReadOnlyList<CardDataInstance> gravePile = complexSystemActionCommand.GetGravePile();

        if (gravePile.Count > 1)
            complexSystemActionCommand.StartCardSelectionMode(SelectCardPileType.Grave, CardSelectionMode.GraveCardsToDeck, 1);
        else
        {
            using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
            Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

            for(int i = 0;i<gravePile.Count;++i)
            {
                writeBuffer[i] = gravePile[i];
            }

            complexSystemActionCommand.GraveCardsToDeck(writeBuffer);

            rentalBuffer.Dispose();
        }

        ResetCommandData();
    }
}