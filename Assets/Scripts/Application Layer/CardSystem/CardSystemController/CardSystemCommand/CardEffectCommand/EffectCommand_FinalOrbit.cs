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

        int usingCnt = 0;
        int extinctionCnt = 0;

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].GetCardData().usingType == UsingType.Nesting &&
                handPile[i].GetCardData().cardType == CardType.Bullet)
            {
                ++usingCnt;
                writeBuffer_Using[i] = handPile[i];
            }
            else
            {
                ++extinctionCnt;
                writeBuffer_Extinction[i] = handPile[i];
            }
        }

        complexSystemActionCommandHandler.CardsToExtinction(writeBuffer_Extinction.Slice(0,extinctionCnt));

        complexSystemActionCommandHandler.CardPileUse(writeBuffer_Using.Slice(0,usingCnt));
        rentalBuffer_Using.Dispose();
        rentalBuffer_Extinction.Dispose();

        ResetCommandData();
    }
}