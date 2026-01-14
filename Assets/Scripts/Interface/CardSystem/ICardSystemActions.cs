using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardSystemActions
{
    public void CardUsed(CardDataInstance usedCard);
    public void CardUsingFinished();
}