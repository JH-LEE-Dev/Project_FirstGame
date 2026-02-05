using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/SpaceShuttle")]
public class EffectCommand_SpaceShuttle : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    public override void InitializeCommand(int _valueModifier,bool _bUpgraded, CardSystemContextType _cardSystemContextType = CardSystemContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _cardSystemContextType);

        cardSystemContextType = CardSystemContextType.GraveCardsToHand;
    }

    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        IReadOnlyList<IReadOnlyList<CardDataInstance>> prevUsedBulletCards = complexSystemActionCommandHandler.GetPrevUsedBulletCards();

        var handPile = complexSystemActionCommandHandler.GetHandPile();

        if(handPile.Count >= SYSTEM_VAR.maxHandPileCount)
        {
            Debug.LogWarning("패로 카드를 옮기지 못했습니다. 패 카드 총량 초과.");
            return;
        }

        using var rentalBuffer = new RentalScope<CardDataInstance>(SYSTEM_VAR.maxDeckPileCount);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int bufferCnt = 0;

        for (int i = 0; i < prevUsedBulletCards.Count; ++i)
        {
            for (int j = 0; j < prevUsedBulletCards[i].Count; ++j)
            {
                if (prevUsedBulletCards[i][j].GetCardData().elementType == ElementType.Rotation)
                {
                    writeBuffer[bufferCnt] = prevUsedBulletCards[i][j];
                    ++bufferCnt;
                }
            }
        }

        if (handPile.Count + bufferCnt > SYSTEM_VAR.maxHandPileCount)
            bufferCnt = SYSTEM_VAR.maxHandPileCount - handPile.Count;

        if (bufferCnt < 0)
        {
            Debug.LogWarning("패로 카드를 이동시키지 못했습니다. 패 총량 초과.");
            return;
        }

        complexSystemActionCommandHandler.GraveCardsToHand(writeBuffer.Slice(0,bufferCnt), cardSystemContextType);

        ResetCommandData();
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {

    }
}