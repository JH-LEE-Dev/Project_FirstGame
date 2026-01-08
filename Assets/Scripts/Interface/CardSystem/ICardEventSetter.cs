using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardEventSetter
{
    event Action HandChangedEvent;
    event Action<CardDataInstance> CardDrawedEvent;
    public event Action<List<CardDataInstance>> CardPileDrawedEvent;
    event Action CardDrawFinishedEvent;
    event Action CardUsingFinishedEvent;
}
