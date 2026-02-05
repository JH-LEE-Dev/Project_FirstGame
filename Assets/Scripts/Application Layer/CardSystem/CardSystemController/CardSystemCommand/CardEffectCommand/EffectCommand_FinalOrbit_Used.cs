using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/CardEffect/Bullet/FinalOrbit_Used")]
public class EffectCommand_FinalOrbit_Used : CardEffectCommand<ICardLogicSystemActionCommandHandler>
{
    public override void InitializeCommand(int _valueModifier, bool _bUpgraded, GameSystemActionContextType _cardSystemContextType = GameSystemActionContextType.MAX)
    {
        base.InitializeCommand(_valueModifier, _bUpgraded, _cardSystemContextType);

        gameSystemActionContext = GameSystemActionContextType.UsedCardsToExtinction;
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
        Debug.Log("AAAA");
        cardLogicSystemActionCommandHandler.CardsRemoveFromHand(writeBuffer);
        cardLogicSystemActionCommandHandler.CardsToExtinction(writeBuffer);

        ResetCommandData();
    }

    protected override void Undo(ICardLogicSystemActionCommandHandler cardLogicSystemActionCommandHandler)
    {
      
    }
}