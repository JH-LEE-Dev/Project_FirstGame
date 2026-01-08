using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSystemProvider
{
    public void CardUsed(CardDataInstance usedCard);
    public void CardUsingFinished();

    //덱에 있는 카드 
    IReadOnlyList<CardDataInstance> deckCards { get; }

    public int GetDeckCnt();

    public int GetHandCnt();

    public int GetGraveCnt();
}