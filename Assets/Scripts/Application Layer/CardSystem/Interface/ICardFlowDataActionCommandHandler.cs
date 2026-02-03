using System.Collections.Generic;

public interface ICardFlowDataActionCommandHandler 
{
    IReadOnlyList<CardDataInstance> GetPrevTurnHandToGraveCards();
}
