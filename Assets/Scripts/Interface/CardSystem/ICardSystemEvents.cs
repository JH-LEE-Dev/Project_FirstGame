using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public interface ICardSystemEvents
{
    event Action CardDrawFinishedEvent;
    event Action CardUsingTurnFinishedEvent;
    event Action<bool> CardUsingVerificationEvent;
}
