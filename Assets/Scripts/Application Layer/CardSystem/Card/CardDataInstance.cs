using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class CardDataInstance
{
    //카드 인스턴스마다 불변인 데이터는 cardData로 캡슐화.
    private CardData cardData;

    //카드 인스턴스마다 가변인 데이터는 CardDataInstance에 노출.
    public bool bUpgrade = false;
    public int valueModifier = 1;
    public bool bPermanent = false;

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

    public void ResetCardData()
    {
        valueModifier = 1;
    }
}
