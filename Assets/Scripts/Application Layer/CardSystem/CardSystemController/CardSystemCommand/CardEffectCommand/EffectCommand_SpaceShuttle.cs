using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/SpaceShuttle")]
public class EffectCommand_SpaceShuttle : CardEffectCommand<IComplexSystemActionCommandHandler>
{
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

        gameSystemActionContext = GameSystemActionContextType.GraveCardsToHand;
    }

    protected override void Execute(IComplexSystemActionCommandHandler _handler)
    {
        IReadOnlyList<IReadOnlyList<CardDataInstance>> prevUsedBulletCards = _handler.cardSlotSystem.GetPrevUsedBulletCards();

        var handPile = _handler.cardLogicSystem.GetHandPile();

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

        _handler.cardLogicSystem.SetCardSystemContext(gameSystemActionContext);
        _handler.cardLogicSystem.GraveCardsToHand(writeBuffer.Slice(0,bufferCnt));
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {

    }
}