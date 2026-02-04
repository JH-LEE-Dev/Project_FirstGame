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
    public IReadOnlyList<ICardDataInstanceProvider> availableCards;
    public bool bForced;

    public CardSelectionModeData(SelectCardPileType _selectCardPileType, CardSelectionMode _selectionMode, int _amount,
        IReadOnlyList<ICardDataInstanceProvider> _availableCards, bool _bForced)
    {
        selectCardPileType = _selectCardPileType;   
        selectionMode = _selectionMode;
        amount = _amount;
        availableCards = _availableCards;
        bForced = _bForced;
    }
}