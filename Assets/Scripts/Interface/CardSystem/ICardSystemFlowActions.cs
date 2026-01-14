using UnityEngine;

public interface ICardSystemFlowActions
{
    void StartCardDrawTurn(int waveIdx);

    void PlayerTurnFinished();
}
