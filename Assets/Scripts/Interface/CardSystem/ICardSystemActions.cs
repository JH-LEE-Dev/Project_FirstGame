using UnityEngine;

public interface ICardSystemActions
{
    void StartCardDrawTurn(int waveIdx);

    void PlayerTurnFinished();
}
