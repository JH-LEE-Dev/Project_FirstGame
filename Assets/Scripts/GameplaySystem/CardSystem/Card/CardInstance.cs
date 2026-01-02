using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInstance : MonoBehaviour, IPointerClickHandler
{
    private CardData cardData;
    public List<CardEffectData> additionalEffectData;
    public event Action<CardInstance> CardUsedEvent;

    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;   
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void AddCardEffect(CardEffectData effect)
    {
        additionalEffectData.Add(effect);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardUsedEvent?.Invoke(this);
    }
}
