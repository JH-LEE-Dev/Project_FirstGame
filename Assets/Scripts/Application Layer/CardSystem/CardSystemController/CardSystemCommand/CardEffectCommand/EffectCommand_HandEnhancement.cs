using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Hand Enhancement")]
public class EffectCommand_HandEnhancement : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    [SerializeField] int upgradeAmount = 1;

    IComplexSystemActionCommandHandler complexSystemActionCommandHandler;

    public override void InitializeCommand(int _nestingCnt, int _upgradeNestingCnt, int _valueModifier, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_nestingCnt, _upgradeNestingCnt, _valueModifier, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.UpgradeCardsFromHand;
    }

    protected override void Execute(IComplexSystemActionCommandHandler _complexSystemActionCommandHandler)
    {
        complexSystemActionCommandHandler = _complexSystemActionCommandHandler;

        IReadOnlyList<CardDataInstance> handPile = complexSystemActionCommandHandler.GetHandPile();

        using var rentalBuffer_Upgrade = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer_Upgrade = rentalBuffer_Upgrade.Span;

        if (nestingCnt != 0)
        {
            if (complexSystemActionCommandHandler.GetHandPile().Count > upgradeAmount)
                complexSystemActionCommandHandler.StartCardSelectionMode(SelectCardPileType.Hand, CardSelectionMode.UpgradeCardsToHand,
                    upgradeAmount * nestingCnt * valueModifier, cardSystemContextType, null, HandleSelectionResult);
            else
            {
                for (int i = 0; i < handPile.Count; ++i)
                {
                    writeBuffer_Upgrade[i] = handPile[i];
                }

                complexSystemActionCommandHandler.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer_Upgrade, cardSystemContextType);
            }
        }

        if (upgradeNestingCnt != 0)
        {
            for (int i = 0; i < handPile.Count; ++i)
            {
                writeBuffer_Upgrade[i] = handPile[i];
            }

            complexSystemActionCommandHandler.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer_Upgrade, cardSystemContextType);
        }

        ResetCommandData();
    }

    private void HandleSelectionResult(List<ICardDataInstanceProvider> _cards)
    {
        using var rentalBuffer = new RentalScope<CardDataInstance>(_cards.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < _cards.Count; ++i)
        {
            writeBuffer[i] = _cards[i] as CardDataInstance;
        }

        complexSystemActionCommandHandler.RequestCardDataControlSystemActionCommand(CardDataControlSystemActionType.CardsUpgraded, writeBuffer, cardSystemContextType);
    }
}