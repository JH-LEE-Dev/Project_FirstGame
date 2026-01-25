using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/SpaceShuttle")]
public class EffectCommand_SpaceShuttle : CardEffectCommand<IComplexSystemActionCommandHandler>
{

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<IReadOnlyList<CardDataInstance>> prevUsedBulletCards = complexSystemActionCommandHandler.GetPrevUsedBulletCards();

        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int bufferCnt = 0;

        for (int i = 0; i < prevUsedBulletCards.Count; ++i)
        {
            for (int j = 0; j < prevUsedBulletCards[i].Count; ++j)
            {
                if (prevUsedBulletCards[i][j].GetCardData().elementType == ElementType.Rotation)
                {
                    ++bufferCnt;
                    writeBuffer[i] = prevUsedBulletCards[i][j];
                }
            }
        }

        complexSystemActionCommandHandler.GraveToHand(writeBuffer.Slice(0,bufferCnt));
        rentalBuffer.Dispose();

        ResetCommandData();
    }
}