using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/FinalOrbit")]
public class EffectCommand_FinalOrbit : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UsedCardsToExtinction;
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == 0)
            return;

        using var rentalBuffer_Using = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Using = rentalBuffer_Using.Span;

        using var rentalBuffer_Extinction = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Extinction = rentalBuffer_Extinction.Span;

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        int usingCnt = 0;
        int extinctionCnt = 0;
        int upgradeCnt = 0;

        for (int i = 0; i < handPile.Count; ++i)
        {
            CardData cardData = handPile[i].GetCardData();

            if (cardData.usingType == UsingType.Nesting &&
                cardData.cardType == CardType.Bullet)
            {
                writeBuffer_Using[usingCnt] = handPile[i];
                ++usingCnt;

                if (upgradeNestingCnt != 0)
                {
                    writeBuffer_Upgrade[upgradeCnt] = handPile[i];
                    ++upgradeCnt;
                }
            }

            writeBuffer_Extinction[extinctionCnt] = handPile[i];
            ++extinctionCnt;
        }

        if (extinctionCnt != 0)
        {
            complexSystemActionCommandHandler.CardsRemoveFromHands(writeBuffer_Extinction.Slice(0, extinctionCnt), cardSystemContextType);
            complexSystemActionCommandHandler.CardsToExtinction(writeBuffer_Extinction.Slice(0, extinctionCnt), cardSystemContextType);

            if (upgradeCnt != 0)
            {
                complexSystemActionCommandHandler.UpgradeCards(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
            }

            complexSystemActionCommandHandler.CardPileUse(writeBuffer_Using.Slice(0, usingCnt), cardSystemContextType);

            if (upgradeCnt != 0)
            {
                complexSystemActionCommandHandler.RevertCardsUpgrade(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
            }
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == 0)
            return;

        using var rentalBuffer_Using = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Using = rentalBuffer_Using.Span;

        using var rentalBuffer_Extinction = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Extinction = rentalBuffer_Extinction.Span;

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        int usingCnt = 0;
        int extinctionCnt = 0;
        int upgradeCnt = 0;

        for (int i = 0; i < handPile.Count; ++i)
        {
            CardData cardData = handPile[i].GetCardData();

            if (cardData.usingType == UsingType.Nesting &&
                cardData.cardType == CardType.Bullet)
            {
                writeBuffer_Using[usingCnt] = handPile[i];
                ++usingCnt;

                if (upgradeNestingCnt != 0)
                {
                    writeBuffer_Upgrade[upgradeCnt] = handPile[i];
                    ++upgradeCnt;
                }
            }

            writeBuffer_Extinction[extinctionCnt] = handPile[i];
            ++extinctionCnt;
        }

        if (extinctionCnt != 0)
        {
            complexSystemActionCommandHandler.CardsRemoveFromHands(writeBuffer_Extinction.Slice(0, extinctionCnt), cardSystemContextType);
            complexSystemActionCommandHandler.CardsToExtinction(writeBuffer_Extinction.Slice(0, extinctionCnt), cardSystemContextType);

            if (upgradeCnt != 0)
            {
                complexSystemActionCommandHandler.UpgradeCards(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
            }

            complexSystemActionCommandHandler.UndoCardPileUse(writeBuffer_Using.Slice(0, usingCnt), cardSystemContextType);

            if (upgradeCnt != 0)
            {
                complexSystemActionCommandHandler.RevertCardsUpgrade(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
            }
        }
    }
}