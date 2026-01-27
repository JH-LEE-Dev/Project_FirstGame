
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

    public CardSelectionModeData(SelectCardPileType _selectCardPileType, CardSelectionMode _selectionMode, int _amount)
    {
        selectCardPileType = _selectCardPileType;   
        selectionMode = _selectionMode;
        amount = _amount;
    }
}