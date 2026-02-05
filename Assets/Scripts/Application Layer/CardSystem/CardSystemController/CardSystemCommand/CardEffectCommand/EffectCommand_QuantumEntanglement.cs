using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/QuantumEntanglement")]
public class EffectCommand_QuantumEntanglement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int duplicateAmount = 1;
    [SerializeField] int upgradedDuplicateAmount = 1;

    private List<CardName> forbiddenCards = new List<CardName>(5);
    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

    private IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _cardSystemContextType);

        if (forbiddenCards.Count == 0)
            forbiddenCards.Add(CardName.QuantumEntanglement);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        availableCards.Clear();

        complexSystemActionCommandHandler = _complexSystemActionCommandHandler;

        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (forbiddenCards.Contains(handPile[i].GetCardData().cardName))
                continue;

            availableCards.Add(handPile[i]);
        }

        if (bUpgraded == false)
        {
            if (availableCards.Count > duplicateAmount * valueModifier)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, duplicateAmount * valueModifier, gameSystemActionContext,
                    availableCards, true, HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < availableCards.Count; ++i)
                {
                    writeBuffer[i] = availableCards[i] as CardDataInstance;
                }

                if (availableCards.Count > 0)
                    complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer, GameSystemActionContextType.MAX);
            }
        }
        else
        {
            if (availableCards.Count > upgradedDuplicateAmount * valueModifier)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, upgradedDuplicateAmount * valueModifier, gameSystemActionContext,
                    availableCards, true, HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                int duplicateCnt = 0;
                for (int i = 0; i < availableCards.Count; ++i)
                {
                    writeBuffer[i] = availableCards[i] as CardDataInstance;
                    ++duplicateCnt;
                }

                if (duplicateCnt != 0)
                    writeBuffer[duplicateCnt] = writeBuffer[duplicateCnt - 1];


                if (availableCards.Count > 0)
                    complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, duplicateCnt + 1), GameSystemActionContextType.MAX);
            }
        }

        ResetCommandData();
    }

    private void HandleCardSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int duplicateCnt = 0;
        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[duplicateCnt] = _cards[i] as CardDataInstance;
            ++duplicateCnt;
        }

        if (bUpgraded == false)
        {
            if (_cards.Count > 0)
                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, _cards.Count), GameSystemActionContextType.MAX);
        }
        else
        {
            if (duplicateCnt != 0)
                writeBuffer[duplicateCnt] = writeBuffer[duplicateCnt - 1];

            if (_cards.Count > 0)
                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, duplicateCnt + 1), GameSystemActionContextType.MAX);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}