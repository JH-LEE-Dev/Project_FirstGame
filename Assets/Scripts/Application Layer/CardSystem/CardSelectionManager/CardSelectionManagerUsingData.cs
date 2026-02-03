using System.Collections.Generic;

public enum CardSelectionMode
{
    DuplicateCardsToHand,
    DuplicateCardsToDeck,
    CardsToExtinction,
    CardsToGrave,
    UpgradeCardsToHand,
    ExtinctionCardsToDeck,
    ExtinctionCardsToHand,
    GraveCardsToDeck,
    GraveCardsToHand,
}

public enum SelectCardPileType
{
    Hand,
    Deck,
    Grave,
    Extinction,
    MAX,
}

public struct CardSelectionModeData
{
    public SelectCardPileType selectCardPileType;
    public CardSelectionMode selectionMode;
    public int amount;
    public List<CardName> forbiddenCards;

    public CardSelectionModeData(SelectCardPileType _selectCardPileType, CardSelectionMode _selectionMode, int _amount,
        List<CardName> _forbiddenCards)
    {
        selectCardPileType = _selectCardPileType;   
        selectionMode = _selectionMode;
        amount = _amount;
        forbiddenCards = _forbiddenCards;
    }
}