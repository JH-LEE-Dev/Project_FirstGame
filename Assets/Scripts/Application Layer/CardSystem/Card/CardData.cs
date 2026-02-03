using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardData : ICardDataProvider
{
    public int id;

    public Sprite cardImage;
    public CardName cardName;
    public int cardNameId;
    public int cardDescriptionId;
    public CardType cardType;
    public ElementType elementType;
    public UsingType usingType;
    public CardEffectPriority priority = CardEffectPriority.Normal;
    public bool bUpgradable = false;

    [Space]
    [Header("Card Effects")]
    public List<CardLogicSystemEffectType> cardLogicSystemEffects;
    public List<CardDataControlSystemEffectType> cardDataControlSystemEffects;
    public List<CardStatusEffectType> cardStatusEffects;
    public List<CardSlotSystemEffectType> cardSlotSystemEffects;
    public List<ComplexSystemEffectType> complexSystemEffects;
    public List<CardSelectionSystemEffectType> selectionSystemEffects;
    public CardEffectCommand HandPileExistEffect;

    bool ICardDataProvider.bUpgradable => bUpgradable;

    int ICardDataProvider.id => id;

    Sprite ICardDataProvider.cardImage => cardImage;

    CardName ICardDataProvider.cardName => cardName;

    int ICardDataProvider.cardNameId => cardNameId;

    int ICardDataProvider.cardDescriptionId => cardDescriptionId;

    CardType ICardDataProvider.cardType => cardType;

    ElementType ICardDataProvider.elementType => elementType;

    UsingType ICardDataProvider.usingType => usingType;
}