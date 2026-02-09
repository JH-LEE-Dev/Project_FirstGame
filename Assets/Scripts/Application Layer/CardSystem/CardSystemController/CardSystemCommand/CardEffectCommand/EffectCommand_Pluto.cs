using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Pluto")]
public class EffectCommand_Pluto : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardName> forbiddenCards = new List<CardName>(5);
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, Dictionary<BulletElementType, BulletElementData> _elementTypes,
      Dictionary<DebuffElementEffectType, DebuffElementData> _debuffTypes,
      GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _elementTypes, _debuffTypes, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.ExtinctionCardsToDeck;

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.Pluto);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        availableCards.Clear();

        complexSystemActionCommandHandler = _complexSystemActionCommandHandler;

        IReadOnlyList<CardDataInstance> extinctionPile = complexSystemActionCommandHandler.GetExtinctionPile();

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
            complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Extinction,
                CardSelectionMode.ExtinctionCardsToDeck, valueModifier, gameSystemActionContext, availableCards, true, HandleCardSelectionResult);
        else
        {
            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer[i] = availableCards[i] as CardDataInstance;
            }

            if (availableCards.Count > 0)
                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.ExtinctionCardsToDeck, writeBuffer, gameSystemActionContext);
        }

        ResetCommandData();
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
            complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.ExtinctionCardsToDeck, writeBuffer, gameSystemActionContext);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}
