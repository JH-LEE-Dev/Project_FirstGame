using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class CardData
{
    public int id;
    public int cost;

    public CardType type;
    public List<CardEffectData> effects;
}