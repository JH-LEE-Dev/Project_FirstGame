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
    CardsUpgraded,
    CardsValueModified,
    ValueModified,
    MAX,
}


public struct CardUIActionData
{
    public CardUIActionType uiActionType;
    public GameSystemActionContextType cardSystemContextType;
    public List<ICardDataInstanceProvider> cards;
}

public struct CardUIActionBatch
{
    public List<CardUIActionData> actionList;
    public int idx;
}
