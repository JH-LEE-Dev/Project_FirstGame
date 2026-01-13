using System;
using UnityEngine;

public interface IWaveSystemEvents
{
    public event Action WaveEndEvent;
    public event Action StartMoveEvent;
}
