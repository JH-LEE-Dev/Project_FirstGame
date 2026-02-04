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
    public IReadOnlyList<ICardDataInstanceProvider> forbiddenCards;
    public bool bForced;

    public CardSelectionModeData(SelectCardPileType _selectCardPileType, CardSelectionMode _selectionMode, int _amount,
        IReadOnlyList<ICardDataInstanceProvider> _forbiddenCards, bool _bForced)
    {
        selectCardPileType = _selectCardPileType;   
        selectionMode = _selectionMode;
        amount = _amount;
        forbiddenCards = _forbiddenCards;
        bForced = _bForced;
    }
}