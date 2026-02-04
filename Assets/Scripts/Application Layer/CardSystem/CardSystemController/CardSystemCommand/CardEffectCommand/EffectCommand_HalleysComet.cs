using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Halley's Comet")]
public class EffectCommand_HalleysComet : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);

    private IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.HalleysComet);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommand)
    {
        complexSystemActionCommandHandler = _complexSystemActionCommand;

        IReadOnlyList<CardDataInstance> gravePile = complexSystemActionCommandHandler.GetGravePile();

        if (gravePile.Count > 1)
            complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Grave,
                CardSelectionMode.GraveCardsToDeck, 1, cardSystemContextType, forbiddenCards,HandleCardSelectionResult);
        else
        {
            using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
            Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

            for(int i = 0;i<gravePile.Count;++i)
            {
                writeBuffer[i] = gravePile[i];
            }

            complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.GraveCardsToDeck, writeBuffer, cardSystemContextType);
        }

        ResetCommandData();
    }

    private void HandleCardSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.GraveCardsToDeck, writeBuffer, cardSystemContextType);
    }
}