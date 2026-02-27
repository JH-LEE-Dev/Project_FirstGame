using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Hand Enhancement")]
public class EffectCommand_HandEnhancement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int upgradeAmount = 1;

    IComplexSystemActionCommandHandler handler;

    private List<ICardDataInstanceProvider> availableCards = new List<ICardDataInstanceProvider>(SYSTEM_VAR.maxDeckPileCount);

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

        gameSystemActionContext = GameSystemActionContextType.UpgradeCardsFromHand;
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        availableCards.Clear();

        handler = _handler;

        IReadOnlyList<CardDataInstance> handPile = handler.cardLogicSystem.GetHandPile();

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].GetCardData().bUpgradable == false || handPile[i].IsUpgraded())
                continue;

            availableCards.Add(handPile[i]);
        }

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        if (bUpgraded == false)
        {
            if (availableCards.Count > upgradeAmount)
                handler.cardSelectionSystem.StartCardSelectionMode(SelectCardPileType.Hand, CardSelectionMode.UpgradeCardsToHand,
                    upgradeAmount, availableCards, true, HandleSelectionResult);
            else
            {
                for (int i = 0; i < availableCards.Count; ++i)
                {
                    writeBuffer_Upgrade[i] = availableCards[i] as CardDataInstance;
                }

                if (availableCards.Count > 0)
                    handler.cardSystem.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer_Upgrade.Slice(0,availableCards.Count), gameSystemActionContext);
            }
        }
        else
        {
            for (int i = 0; i < availableCards.Count; ++i)
            {
                writeBuffer_Upgrade[i] = availableCards[i] as CardDataInstance;
            }

            if (availableCards.Count > 0)
                handler.cardSystem.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer_Upgrade, gameSystemActionContext);
        }
    }

    private void HandleSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        if (_cards.Count > 0)
            handler.cardSystem.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer, gameSystemActionContext);
    }

    protected override void Undo(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {

    }
}