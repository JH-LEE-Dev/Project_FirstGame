using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardDeckDataBase", menuName = "Game/Card Deck DataBase")]
public class CardDeckDataBase : ScriptableObject
{
    public List<CardPileData> cardPileData;

    public CardPileData GetCardPileData(int id)
    {
        return cardPileData[id];
    }
}

[Serializable]
public struct CardPileData
{
    public CardName cardName;
    public int cnt;
}
