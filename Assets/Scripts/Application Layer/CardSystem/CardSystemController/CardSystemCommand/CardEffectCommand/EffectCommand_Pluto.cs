using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Pluto")]
public class EffectCommand_Pluto : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);

    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.ExtinctionCardsToDeck;

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.Pluto);
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> extinctionPile = complexSystemActionCommandHandler.GetExtinctionPile();

        if (extinctionPile.Count == 0)
            return;

        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (extinctionPile.Count > 1)
            complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Extinction,
                CardSelectionMode.ExtinctionCardsToDeck, 1 * nestingCnt * valueModifier, cardSystemContextType,forbiddenCards);
        else
        {
            for (int i = 0; i < extinctionPile.Count; ++i)
            {
                writeBuffer[i] = extinctionPile[i];
            }

            complexSystemActionCommandHandler.ExtinctionCardsToDeck(writeBuffer, cardSystemContextType);
        }

        ResetCommandData();
    }
}
