using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDeckProvider
{
    public bool CardUsed(CardInstance usedCard);
    public void CardUsingFinished();

    // 현재 손에 들고 있는 카드 데이터 (읽기 전용)
    IReadOnlyList<CardData> HandCards { get; }

    // 덱에 남은 카드 수, 묘지 카드 수 등 UI에 표시할 정보
    int deckCnt { get; }
    int graveCnt { get; }

    int curCost { get; }

    // 데이터가 변경되었을 때 UI에 알릴 이벤트
    event Action HandChangedEvent;

    event Action<CardInstance> CardDrawedEvent;
    event Action CardDrawFinishedEvent;
    event Action CardUsingFinishedEvent;
}