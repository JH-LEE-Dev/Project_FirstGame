using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Scan")]
public class EffectCommand_Scan : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] private int selectCnt = 1;
    [SerializeField] private int upgradedSelectCnt = 2;

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> gravePile = complexSystemActionCommandHandler.GetGravePile();

        if (nestingCnt != 0)
        {
            if(gravePile.Count > selectCnt)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Grave, CardSelectionMode.GraveCardsToHand, selectCnt);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0;i<gravePile.Count;++i)
                {
                    writeBuffer[i] = gravePile[i];
                }

                complexSystemActionCommandHandler.GraveCardsToHand(writeBuffer);

                rentalBuffer.Dispose();
            }
        }

        if (upgradeNestingCnt != 0)
        {
            if (gravePile.Count > upgradedSelectCnt)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Grave, CardSelectionMode.GraveCardsToHand, upgradedSelectCnt);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < gravePile.Count; ++i)
                {
                    writeBuffer[i] = gravePile[i];
                }

                complexSystemActionCommandHandler.GraveCardsToHand(writeBuffer);

                rentalBuffer.Dispose();
            }
        }

        ResetCommandData();
    }
}