using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardEventSetter
{
    event Action<CardDataInstance> CardDrawedEvent;
    event Action<List<CardDataInstance>> CardPileDrawedEvent;
    event Action CardDrawFinishedEvent;
    event Action CardUsingTurnFinished;
    event Action<bool> CardUsingVerificationEvent;
}
