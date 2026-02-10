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
    public List<BulletElementData> defaultElementTypes = new List<BulletElementData>();
    public List<DebuffElementData> defaultdebuffTypes = new List<DebuffElementData>();

    [Space]
    [Header("Card Effects")]
    public List<CardEffectCommand> cardLogicSystemEffects_Prefab;
    public List<CardEffectCommand> cardDataControlSystemEffects_Prefab;
    public List<CardEffectCommand> cardStatusEffects_Prefab;
    public List<CardEffectCommand> cardSlotSystemEffects_Prefab;
    public List<CardEffectCommand> complexSystemEffects_Prefab;
    public List<CardEffectCommand> selectionSystemEffects_Prefab;
    public CardEffectCommand HandPileExistEffect_Prefab;

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