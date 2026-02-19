using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/OffenseReorder")]
public class EffectCommand_OffenseReorder : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes,
      Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes,
      GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.HandCardsToDeck;
    }

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        availableCards.Clear();

        complexSystemActionCommandHandler = _complexSystemActionCommandHandler;

        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == 0)
            return;

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].GetCardData().cardType == CardType.Inherence)
                availableCards.Add(handPile[i]);
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(availableCards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (availableCards.Count > 1)
            complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                CardSelectionMode.HandCardsToDeck, valueModifier, gameSystemActionContext, availableCards, true, HandleCardSelectionResult);
        else
        {
            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
            }

            if (availableCards.Count > 0)
            {
                if (bUpgraded)
                    complexSystemActionCommandHandler.AdditionalDraw(2, GameSystemActionContextType.MAX);
                else
                    complexSystemActionCommandHandler.AdditionalDraw(1, GameSystemActionContextType.MAX);

                complexSystemActionCommandHandler.CardsRemoveFromHands(writeBuffer, GameSystemActionContextType.UsedCardsRemoveFromHand);
                complexSystemActionCommandHandler.CardsToDeck(writeBuffer,gameSystemActionContext);
            }
        }
    }

    private void HandleCardSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        if (bUpgraded)
            complexSystemActionCommandHandler.AdditionalDraw(2, GameSystemActionContextType.MAX);
        else
            complexSystemActionCommandHandler.AdditionalDraw(1, GameSystemActionContextType.MAX);

        complexSystemActionCommandHandler.CardsRemoveFromHands(writeBuffer, GameSystemActionContextType.UsedCardsRemoveFromHand);
        complexSystemActionCommandHandler.CardsToDeck(writeBuffer, gameSystemActionContext);
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {

    }
}
