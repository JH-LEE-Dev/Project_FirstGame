using UnityEngine;

public interface ICardSystemActionCommandHandler
{
    void StartCardPileDraw();
    void DrawAgain(int drawAmount);
    void ApplyValueModifier(int valueModifier);
}
