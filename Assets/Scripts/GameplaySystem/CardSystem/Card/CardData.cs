using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class CardData
{
    public string id;
    public GameObject cardObject;
    public int cost;

    public CardType type;
    public List<CardEffectData> effects;
}