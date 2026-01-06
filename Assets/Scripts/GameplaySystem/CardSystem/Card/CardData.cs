using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class CardData
{
    public int id;
    public int cost; // 지우셈

    public CardType cardType;
    public ElementType elementType;
    public UsingType usingType;

    public bool bUpgrade = false;
    public bool bFlash = false; // 한 턴 지나면 사라질 놈 (인게임중에 다른 카드 효과에 의해 만들어진놈)

    public List<CardEffectData> effects;
}