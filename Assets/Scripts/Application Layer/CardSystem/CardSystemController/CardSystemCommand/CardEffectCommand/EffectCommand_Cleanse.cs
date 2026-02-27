using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Cleanse")]
public class EffectCommand_Cleanse : CardEffectCommand<IComplexSystemActionCommandHandler>
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

    protected override void Execute(IComplexSystemActionCommandHandler handler)
    {
        bool bCleansed = false;

        var enemies = handler.statusSystem.GetEnemyHandlers();

        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i].enemyData.currentAppliedDebuff.Count != 0)
            {
                bCleansed = true;
                enemies[i].ClearDebuff();
            }
        }

        var player = handler.statusSystem.GetPlayerHandler();

        if (player.playerData.currentAppliedDebuff.Count != 0)
        {
            bCleansed = true;
            player.ClearDebuff();
        }

        var handPile = handler.cardLogicSystem.GetHandPile();

        using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        int cardCnt = 0;

        for (int i = 0; i < handPile.Count; ++i)
        {
            if (handPile[i].GetCardData().cardType == CardType.Debuff)
            {
                writeBuffer[cardCnt] = handPile[i];
                ++cardCnt;
            }
        }

        if (cardCnt != 0)
        {
            bCleansed = true;
            handler.cardLogicSystem.CardsRemoveFromHand(writeBuffer.Slice(0, cardCnt));
            handler.cardLogicSystem.SetCardSystemContext(GameSystemActionContextType.UsedCardsToExtinction);
            handler.cardLogicSystem.CardsToExtinction(writeBuffer.Slice(0, cardCnt));
        }

        if (bCleansed)
        {
            if (bUpgraded == false)
                handler.cardLogicSystem.DrawAgain(2);
            else
                handler.cardLogicSystem.DrawAgain(3);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {

    }
}