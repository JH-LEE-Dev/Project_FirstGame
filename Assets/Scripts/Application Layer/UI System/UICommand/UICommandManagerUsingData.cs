using System.Collections.Generic;

public enum ActionType_CardSystem
{
    PileDraw,
    AdditionalDraw,
    GraveToDeck,
    HandToGrave,
    UsedCardToExtinction,
    UsedCardToGrave,
    ExtinctionToDeck,
    GraveToHand,
}

public struct ActionData_CardSystem
{
    public ActionType_CardSystem actionDataType;
    public List<CardDataInstance> cards;
}

public struct ActionDataBatch_CardSystem
{
    public List<ActionData_CardSystem> actionDataList;
    public int idx;
}
