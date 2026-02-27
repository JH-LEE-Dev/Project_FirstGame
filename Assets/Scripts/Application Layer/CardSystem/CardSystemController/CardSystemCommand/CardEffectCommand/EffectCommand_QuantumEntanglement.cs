using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/QuantumEntanglement")]
public class EffectCommand_QuantumEntanglement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int duplicateAmount = 1;
    [SerializeField] int upgradedDuplicateAmount = 1;

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
            forbiddenCards.Add(CardName.QuantumEntanglement);
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> handPile = handler.cardLogicSystem.GetHandPile();

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (forbiddenCards.Contains(handPile[i].GetCardData().cardName))
                continue;

            availableCards.Add(handPile[i]);
        }

        if (bUpgraded == false)
        {
            if (availableCards.Count > duplicateAmount)
                handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, duplicateAmount,
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
                    handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0,availableCards.Count), GameSystemActionContextType.MAX);
            }
        }
        else
        {
            if (availableCards.Count > upgradedDuplicateAmount)
                handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Hand,
                    CardSelectionMode.DuplicateCardsToHand, upgradedDuplicateAmount,
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


                if (duplicateCnt > 0)
                    handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, duplicateCnt + 1), GameSystemActionContextType.MAX);
            }
        }
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
                handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, _cards.Count), GameSystemActionContextType.MAX);
        }
        else
        {
            if (duplicateCnt != 0)
                writeBuffer[duplicateCnt] = writeBuffer[duplicateCnt - 1];

            if (duplicateCnt > 0)
                handler.cardSystem.RequestCardLogicSystemActionCommand(CardLogicSystemActionType.DuplicateCardsToHand, writeBuffer.Slice(0, duplicateCnt + 1), GameSystemActionContextType.MAX);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}