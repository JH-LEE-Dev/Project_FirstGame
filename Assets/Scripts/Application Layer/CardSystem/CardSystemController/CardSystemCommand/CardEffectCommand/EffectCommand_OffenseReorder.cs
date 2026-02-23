using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/OffenseReorder")]
public class EffectCommand_OffenseReorder : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    IComplexSystemActionCommandHandler handler;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes,
      Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes,
      GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.HandCardsToDeck;
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> handPile = handler.cardLogicSystem.GetHandPile();

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
            handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Hand,
                CardSelectionMode.HandCardsToDeck, valueModifier, availableCards, true, HandleCardSelectionResult);
        else
        {
            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
            }

            if (availableCards.Count > 0)
            {
                if (bUpgraded)
                    handler.cardLogicSystem.DrawAgain(2);
                else
                    handler.cardLogicSystem.DrawAgain(1);

                handler.cardLogicSystem.SetCardSystemContext(GameSystemActionContextType.UsedCardsRemoveFromHand);
                handler.cardLogicSystem.CardsRemoveFromHand(writeBuffer.Slice(0,availableCards.Count));
                handler.cardLogicSystem.SetCardSystemContext(gameSystemActionContext);
                handler.cardLogicSystem.CardsToDeck(writeBuffer.Slice(0,availableCards.Count));
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
            handler.cardLogicSystem.DrawAgain(2);
        else
            handler.cardLogicSystem.DrawAgain(1);

        handler.cardLogicSystem.SetCardSystemContext(gameSystemActionContext);
        handler.cardLogicSystem.CardsToDeck(writeBuffer.Slice(0,_cards.Count));
        handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.UsedCardsRemoveFromHand,writeBuffer, GameSystemActionContextType.UsedCardsRemoveFromHand);
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {

    }
}
