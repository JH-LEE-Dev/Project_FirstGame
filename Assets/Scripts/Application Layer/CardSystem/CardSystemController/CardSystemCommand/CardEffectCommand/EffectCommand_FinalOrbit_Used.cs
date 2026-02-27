using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/FinalOrbit_Used")]
public class EffectCommand_FinalOrbit_Used : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(ICardEffectData _cardEffectData,
      GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_cardEffectData, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.UsedCardsToExtinction;
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

    protected override void Execute(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
        IReadOnlyList<CardDataInstance> handPile = cardLogicSystemActionCommandHandler.GetHandPile();

        if (handPile.Count == 0)
            return;

        using var rentalBuffer = new RentalScope<CardDataInstance>(handPile.Count);
        Span<CardDataInstance> writeBuffer = rentalBuffer.Span;

        for (int i = 0; i < handPile.Count; ++i)
        {
            writeBuffer[i] = handPile[i];
        }

        cardLogicSystemActionCommandHandler.CardsRemoveFromHand(writeBuffer.Slice(0,handPile.Count));
        cardLogicSystemActionCommandHandler.CardsToExtinction(writeBuffer.Slice(0,handPile.Count));
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
      
    }
}