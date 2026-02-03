using UnityEngine;

public interface ICardDataProvider
{
    public int id { get; }

    public Sprite cardImage { get; }
    public CardName cardName { get; }
    public int cardNameId { get; }
    public int cardDescriptionId { get; }
    public CardType cardType { get; }
    public ElementType elementType { get; }
    public UsingType usingType { get; }

    public bool bUpgradable { get; }
}
