using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Halley's Comet")]
public class EffectCommand_HalleysComet : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);

    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.HalleysComet);
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommand)
    {
        IReadOnlyList<CardDataInstance> gravePile = complexSystemActionCommand.GetGravePile();

        if (gravePile.Count > 1)
            complexSystemActionCommand.StartCardSelectionMode(SelectCardPileType.Grave, CardSelectionMode.GraveCardsToDeck, 1, cardSystemContextType, forbiddenCards);
        else
        {
            using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
            Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

            for(int i = 0;i<gravePile.Count;++i)
            {
                writeBuffer[i] = gravePile[i];
            }

            complexSystemActionCommand.GraveCardsToDeck(writeBuffer, cardSystemContextType);
        }

        ResetCommandData();
    }
}