using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataBase", menuName = "Game/Card DataBase")]
public class CardDataBase : ScriptableObject
{
    public List<CardData> cardData;

    public CardData GetCardData(int id)
    {
        return cardData[id];
    }
}