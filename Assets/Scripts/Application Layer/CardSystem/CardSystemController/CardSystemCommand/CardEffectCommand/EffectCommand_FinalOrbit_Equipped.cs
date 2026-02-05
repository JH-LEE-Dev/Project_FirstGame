using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/FinalOrbit_Equipped")]
public class EffectCommand_FinalOrbit_Equipped : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<CardDataInstance> currentHandPiles = new List<CardDataInstance>(SYSTEM_VAR.maxHandPileCount);

    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UsedCardsToExtinction;
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        currentHandPiles.Clear();

        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        for(int i = 0;i<handPile.Count;++i)
        {
            currentHandPiles.Add(handPile[i]);
        }

        if (handPile.Count == 0)
            return;

        using var rentalBuffer_Using = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Using = rentalBuffer_Using.Span;

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        int usingCnt = 0;
        int upgradeCnt = 0;

        for (int i = 0; i < handPile.Count; ++i)
        {
            CardData cardData = handPile[i].GetCardData();

            if (cardData.usingType == UsingType.Nesting &&
                cardData.cardType == CardType.Bullet)
            {
                writeBuffer_Using[usingCnt] = handPile[i];
                ++usingCnt;

                if (bUpgraded)
                {
                    writeBuffer_Upgrade[upgradeCnt] = handPile[i];
                    ++upgradeCnt;
                }
            }
        }

        if (upgradeCnt != 0)
        {
            complexSystemActionCommandHandler.UpgradeCards(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
        }

        complexSystemActionCommandHandler.UseCards_AfterAttackEffects(writeBuffer_Using.Slice(0, usingCnt), cardSystemContextType);

        if (upgradeCnt != 0)
        {
            complexSystemActionCommandHandler.RevertCardsUpgrade(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
        }

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        if (currentHandPiles.Count == 0)
            return;

        using var rentalBuffer_Using = new RentalScope<CardDataInstance>(currentHandPiles.Count);
        Span<CardDataInstance> writeBuffer_Using = rentalBuffer_Using.Span;

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(currentHandPiles.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        int usingCnt = 0;
        int upgradeCnt = 0;

        for (int i = 0; i < currentHandPiles.Count; ++i)
        {
            CardData cardData = currentHandPiles[i].GetCardData();

            if (cardData.usingType == UsingType.Nesting &&
                cardData.cardType == CardType.Bullet)
            {
                writeBuffer_Using[usingCnt] = currentHandPiles[i];
                ++usingCnt;

                if (bUpgraded)
                {
                    writeBuffer_Upgrade[upgradeCnt] = currentHandPiles[i];
                    ++upgradeCnt;
                }
            }
        }

        if (upgradeCnt != 0)
        {
            complexSystemActionCommandHandler.UpgradeCards(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
        }

        complexSystemActionCommandHandler.UndoCardPileUse(writeBuffer_Using.Slice(0, usingCnt), cardSystemContextType);

        if (upgradeCnt != 0)
        {
            complexSystemActionCommandHandler.RevertCardsUpgrade(writeBuffer_Upgrade.Slice(0, upgradeCnt), false, cardSystemContextType);
        }

        currentHandPiles.Clear();
    }
}