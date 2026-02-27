using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Halley's Comet")]
public class EffectCommand_HalleysComet : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    private IComplexSystemActionCommandHandler handler;

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

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.HalleysComet);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> gravePile = handler.cardLogicSystem.GetGravePile();

        for (int i = 0; i < gravePile.Count; ++i)
        {
            if (forbiddenCards.Contains(gravePile[i].GetCardData().cardName))
                continue;

            availableCards.Add(gravePile[i]);
        }

        if (availableCards.Count > 1)
            handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Grave,
                CardSelectionMode.GraveCardsToDeck, 1, availableCards, true, HandleCardSelectionResult);
        else
        {
            using var rentalBuffer = new RentalScope<CardDataInstance>(gravePile.Count);
            Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
            }

            if (availableCards.Count > 0)
                handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToDeck, writeBuffer.Slice(0, availableCards.Count), gameSystemActionContext);
        }
    }

    private void HandleCardSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        if (_cards.Count > 0)
            handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.GraveCardsToDeck, writeBuffer, gameSystemActionContext);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommand)
    {

    }
}