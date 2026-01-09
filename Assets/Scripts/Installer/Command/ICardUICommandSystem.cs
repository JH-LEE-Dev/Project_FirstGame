using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICardUICommandSystem
{
    void CreateCommand(JobType_CardSystemUI jobType, ReadOnlySpan<CardDataInstance> cards = default);

    void DispatchCommand();
}
