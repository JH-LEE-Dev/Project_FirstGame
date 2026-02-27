using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Pluto")]
public class EffectCommand_Pluto : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    IComplexSystemActionCommandHandler handler;

    public override bool EffectConditionCheck()
    {
        int newCondition = 0;

        if (newCondition != condition)
        {
            CheckApplyCondition();
            condition = newCondition;
        }
        return true;
    }

    public override void InitializeCommand(ICardEffectData _cardEffectData,
      GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_cardEffectData, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.ExtinctionCardsToDeck;

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.Pluto);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> extinctionPile = handler.cardLogicSystem.GetExtinctionPile();

        if (extinctionPile.Count == 0)
            return;

        for (int i = 0; i < extinctionPile.Count; ++i)
        {
            if (forbiddenCards.Contains(extinctionPile[i].GetCardData().cardName))
                continue;

            availableCards.Add(extinctionPile[i]);
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(1);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        if (availableCards.Count > 1)
            handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Extinction,
                CardSelectionMode.ExtinctionCardsToDeck, 1, availableCards, true, HandleCardSelectionResult);
        else
        {
            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
            }

            if (availableCards.Count > 0)
                handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.ExtinctionCardsToDeck, writeBuffer, gameSystemActionContext);
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

        if (_cards.Count > 0)
            handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.ExtinctionCardsToDeck, writeBuffer, gameSystemActionContext);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}
