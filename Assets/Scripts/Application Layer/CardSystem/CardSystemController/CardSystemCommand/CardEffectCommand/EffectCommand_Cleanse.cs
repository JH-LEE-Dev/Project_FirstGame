using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Magic/Cleanse")]
public class EffectCommand_Cleanse : CardEffectCommand<IComplexSystemActionCommandHandler>
{
    protected override void Execute(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {
        bool bCleansed = false;

        var enemies = complexSystemActionCommandHandler.GetEnemyHandlers();

        for (int i = 0; i < enemies.Count; ++i)
        {
            if (enemies[i].currentAppliedDebuff.Count != 0)
            {
                bCleansed = true;
                enemies[i].ClearDebuff();
            }
        }

        var player = complexSystemActionCommandHandler.GetPlayerHandler();

        if (player.currentAppliedDebuff.Count != 0)
        {
            bCleansed = true;
            player.ClearDebuff();
        }

        var handPile = complexSystemActionCommandHandler.GetHandPile();

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
            complexSystemActionCommandHandler.CardsRemoveFromHands(writeBuffer.Slice(0, cardCnt), GameSystemActionContextType.MAX);
            complexSystemActionCommandHandler.CardsToExtinction(writeBuffer.Slice(0, cardCnt), GameSystemActionContextType.UsedCardsToExtinction);
        }

        if (bCleansed)
        {
            if (bUpgraded == false)
                complexSystemActionCommandHandler.AdditionalDraw(2, GameSystemActionContextType.MAX);
            else
                complexSystemActionCommandHandler.AdditionalDraw(3, GameSystemActionContextType.MAX);
        }
    }

    protected override void Undo(IComplexSystemActionCommandHandler complexSystemActionCommandHandler)
    {

    }
}