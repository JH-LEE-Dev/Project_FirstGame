using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class CardDataInstance : IPointerClickHandler
{
    public event Action<CardDataInstance> CardUsedEvent;

    private CardData cardData;
    public bool bUpgrade = false;
    public void Initialize(CardData cardData)
    {
        this.cardData = cardData;
        ResetState();
    }

    public void ResetState()
    {
        // 사용 중 변한 값 전부 초기화
        // 예: cost, cooldown, tempModifier 등
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void AddCardSystemEffect(CardSystemEffectType effectType)
    {
        cardData.cardSystemEffects.Add(effectType);
    }

    public void AddCardStatusEffect(CardStatusEffectType effectType)
    {
        cardData.cardStatusEffects.Add(effectType);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardUsedEvent?.Invoke(this);
    }
}
