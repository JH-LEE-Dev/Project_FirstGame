using System;
using UnityEngine;

public interface ICardSystemActionCommandHandler : ICommandHandler
{
    void StartCardPileDraw();
    void DrawAgain(int drawAmount);
    void ApplyValueModifier(int valueModifier);

    bool DeckConditionCheck(int cardID);

    void GraveToHand(ReadOnlySpan<CardDataInstance> graveToDeckCards);
}
