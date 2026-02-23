using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/BattlePrep")]
public class EffectCommand_BattlePrep : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    IComplexSystemActionCommandHandler handler;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes,
      Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes,
      GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.GraveCardsToHand;
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> gravePile = handler.cardLogicSystem.GetGravePile();

        if (gravePile.Count == 0)
            return;

        for (int i = 0; i < gravePile.Count; ++i)
        {
            if (gravePile[i].GetCardData().cardType == CardType.Inherence)
                availableCards.Add(gravePile[i]);
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(availableCards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (availableCards.Count > 1)
            handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Grave,
                CardSelectionMode.GraveCardsToHand, valueModifier, availableCards, true, HandleCardSelectionResult);
        else
        {
            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
            }

            if (bUpgraded)
                handler.cardSystem.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer, gameSystemActionContext);

            if (availableCards.Count > 0)
                handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, gameSystemActionContext);
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
            handler.cardSystem.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer, gameSystemActionContext);

        if (_cards.Count > 0)
            handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToHand, writeBuffer, gameSystemActionContext);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}