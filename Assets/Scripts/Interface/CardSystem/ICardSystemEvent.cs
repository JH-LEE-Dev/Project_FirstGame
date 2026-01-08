using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public interface ICardSystemEvent
{
    event Action<CardDataInstance> CardDrawedEvent;
    event Action<List<CardDataInstance>> CardPileDrawedEvent;
    event Action CardDrawFinishedEvent;
    event Action CardUsingTurnFinishedEvent;
    event Action<bool> CardUsingVerificationEvent;
}
