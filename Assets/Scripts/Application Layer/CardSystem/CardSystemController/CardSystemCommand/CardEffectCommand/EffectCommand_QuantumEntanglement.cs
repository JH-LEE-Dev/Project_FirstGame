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

    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

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

        if (nestingCnt != 0)
        {
            if (handPile.Count > duplicateAmount * nestingCnt * valueModifier)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, duplicateAmount * nestingCnt * valueModifier, cardSystemContextType,
                    availableCards,true, HandleCardSelectionResult);
            else
            {
                using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
                Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

                for (int i = 0; i < availableCards.Count; ++i)
                {
                    writeBuffer[i] = availableCards[i] as CardDataInstance;
                }

                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer, CardSystemContextType.MAX);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            if (handPile.Count > upgradedDuplicateAmount * upgradeNestingCnt * valueModifier)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, upgradedDuplicateAmount * upgradeNestingCnt * valueModifier, cardSystemContextType,
                    availableCards,true, HandleCardSelectionResult);
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
                ++duplicateCnt;
                if (duplicateCnt != 0)
                    writeBuffer[duplicateCnt] = writeBuffer[duplicateCnt - 1];

                complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, duplicateCnt), CardSystemContextType.MAX);
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

        if (nestingCnt != 0)
        {
            complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, _cards.Count), CardSystemContextType.MAX);
        }

        if (upgradeNestingCnt != 0)
        {
            if (duplicateCnt != 0)
                writeBuffer[duplicateCnt] = writeBuffer[duplicateCnt - 1];
            complexSystemActionCommandHandler.RequestCardSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, duplicateCnt+1), CardSystemContextType.MAX);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}