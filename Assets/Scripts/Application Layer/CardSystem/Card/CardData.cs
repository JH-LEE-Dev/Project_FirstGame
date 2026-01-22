using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class CardData
{
    public int id;

    public Sprite cardImage;
    public CardName cardName;
    public CardType cardType;
    public ElementType elementType;
    public UsingType usingType;
    public CardEffectPriority priority = CardEffectPriority.Normal;
    [Multiline(3)] public string cardDescription;

    public bool bFlash = false; // 한 턴 지나면 사라질 놈 (인게임중에 다른 카드 효과에 의해 만들어진놈)

    public List<CardSystemEffectType> cardSystemEffects;
    public List<CardStatusEffectType> cardStatusEffects;
    public List<CardSlotSystemEffectType> cardSlotSystemEffects;
    public List<ComplexSystemEffectType> complexSystemEffects;
}