using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Pluto")]
public class EffectCommand_Pluto : CardEffectCommand<ICardSystemActionCommandHandler>
{
    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.ExtinctionCardsToDeck;
    }

    protected override void Execute(ICardSystemActionCommandHandler cardSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> extinctionPile = cardSystemActionCommandHandler.GetExtinctionPile();

        if (extinctionPile.Count == 0)
            return;

        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int randomIdx = UnityEngine.Random.Range(0, extinctionPile.Count - 1);

        writeBuffer[0] = extinctionPile[randomIdx];

        cardSystemActionCommandHandler.ExtinctionCardsToDeck(writeBuffer);

        rentalBuffer.Dispose();

        ResetCommandData();
    }
}
