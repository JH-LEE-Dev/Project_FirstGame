
public enum CardSelectionMode
{
    DuplicateToHand,
    DuplicateToDeck,
    ToExtinction,
    ToGrave,
    UpgradeToHand,
}

public struct CardSelectionModeData
{
    public CardSelectionMode selectionMode;
    public int amount;

    public CardSelectionModeData(CardSelectionMode _selectionMode, int _amount)
    {
        selectionMode = _selectionMode;
        amount = _amount;
    }
}