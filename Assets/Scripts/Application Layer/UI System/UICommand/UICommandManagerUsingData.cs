using System.Collections.Generic;

public enum CardUIActionType
{
    PileDraw,
    AdditionalDraw,
    HandCardsToGrave,
    GraveCardsToDeck,
    GraveCardsToHand,
    CardsToExtinction,
    CardsToGrave,
    ExtinctionCardsToDeck,
    CardsToHand,
    CardsToDeck,
    MAX,
}


public struct CardUIActionData
{
    public CardUIActionType uiActionType;
    public CardSystemContextType cardSystemContextType;
    public List<CardDataInstance> cards;
}

public struct CardUIActionBatch
{
    public List<CardUIActionData> actionList;
    public int idx;
}
