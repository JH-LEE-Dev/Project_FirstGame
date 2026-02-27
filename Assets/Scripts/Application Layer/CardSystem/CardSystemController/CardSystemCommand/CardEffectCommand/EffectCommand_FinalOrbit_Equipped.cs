using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/FinalOrbit_Equipped")]
public class EffectCommand_FinalOrbit_Equipped : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    private List<FinalOrbitCardData> currentUsingPiles = new List<FinalOrbitCardData>(SYSTEM_VAR.maxHandPileCount);

    private struct FinalOrbitCardData
    {
        public CardDataInstance card;
        public bool bCardUpgraded;
        public FinalOrbitCardData(CardDataInstance _card, bool _bCardUpgraded)
        {
            card = _card;
            bCardUpgraded = _bCardUpgraded;
        }
    }

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

        gameSystemActionContext = GameSystemActionContextType.UsedCardsToExtinction;
    }

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        EffectConditionCheck();

        currentUsingPiles.Clear();

        IReadOnlyList<CardDataInstance> handPile = handler.cardLogicSystem.GetHandPile();

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

                currentUsingPiles.Add(new FinalOrbitCardData(handPile[i], handPile[i].IsUpgraded()));

                if (bUpgraded && handPile[i].IsUpgraded() == false)
                {
                    writeBuffer_Upgrade[upgradeCnt] = handPile[i];
                    ++upgradeCnt;
                }
            }
        }

        if (upgradeCnt != 0)
        {
            handler.cardDataSystem.UpgradeCards(writeBuffer_Upgrade.Slice(0, upgradeCnt), false);
        }

        handler.cardSystem.UseCards_AfterAttackEffects(writeBuffer_Using.Slice(0, usingCnt));

        if (upgradeCnt != 0)
        {
            handler.cardDataSystem.RevertCardsUpgrade(writeBuffer_Upgrade.Slice(0, upgradeCnt), false);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler handler)
    {
        if (currentUsingPiles.Count == 0)
            return;

        using var rentalBuffer_Using = new RentalScope<CardDataInstance>(currentUsingPiles.Count);
        Span<CardDataInstance> writeBuffer_Using = rentalBuffer_Using.Span;

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(currentUsingPiles.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        int usingCnt = 0;
        int upgradeCnt = 0;

        for (int i = 0; i < currentUsingPiles.Count; ++i)
        {
            writeBuffer_Using[usingCnt] = currentUsingPiles[i].card;
            ++usingCnt;

            if (bUpgraded && currentUsingPiles[i].card.IsUpgraded() == false)
            {
                writeBuffer_Upgrade[upgradeCnt] = currentUsingPiles[i].card;
                ++upgradeCnt;
            }
        }

        //이 카드를 장착한 후에 강화된 카드가 있으면, 장착 전으로 돌아가야 함.
        using var rentalBuffer_Revert = new RentalScope<CardDataInstance>(currentUsingPiles.Count);
        Span<CardDataInstance> writeBuffer_Revert = rentalBuffer_Revert.Span;

        int revertCurrentCardsCnt = 0;

        if (bUpgraded == false)
        {
            for (int i = 0; i < currentUsingPiles.Count; ++i)
            {
                if (currentUsingPiles[i].card.IsUpgraded() == true)
                {
                    if (currentUsingPiles[i].bCardUpgraded == false)
                    {
                        writeBuffer_Revert[revertCurrentCardsCnt] = currentUsingPiles[i].card;
                        ++revertCurrentCardsCnt;
                    }
                }
            }
        }

        if (revertCurrentCardsCnt != 0 && bUpgraded == false)
            handler.cardDataSystem.RevertCardsUpgrade(writeBuffer_Revert.Slice(0, revertCurrentCardsCnt), false);

        if (upgradeCnt != 0)
        {
            handler.cardDataSystem.UpgradeCards(writeBuffer_Upgrade.Slice(0, upgradeCnt), false);
        }

        handler.cardSystem.UndoUseCards_AfterAttackEffects(writeBuffer_Using.Slice(0, usingCnt));

        if (upgradeCnt != 0)
        {
            handler.cardDataSystem.RevertCardsUpgrade(writeBuffer_Upgrade.Slice(0, upgradeCnt), false);
        }

        if (revertCurrentCardsCnt != 0 && bUpgraded == false)
            handler.cardDataSystem.UpgradeCards(writeBuffer_Revert.Slice(0, revertCurrentCardsCnt), false);

        currentUsingPiles.Clear();
    }
}