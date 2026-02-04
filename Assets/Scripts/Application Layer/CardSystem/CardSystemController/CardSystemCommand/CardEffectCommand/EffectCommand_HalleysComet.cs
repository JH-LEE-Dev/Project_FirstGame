using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Halley's Comet")]
public class EffectCommand_HalleysComet : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

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

        for (int i = 0; i < gravePile.Count; ++i)
        {
            if (forbiddenCards.Contains(gravePile[i].GetCardData().cardName))
                continue;

            availableCards.Add(gravePile[i]);
        }

        if (gravePile.Count > 1)
            complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Grave,
                CardSelectionMode.GraveCardsToDeck, 1, cardSystemContextType, availableCards, HandleCardSelectionResult);
        else
        {
            using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
            Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

            for(int i = 0;i< availableCards.Count;++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
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