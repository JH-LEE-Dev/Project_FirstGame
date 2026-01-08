using System;
using UnityEngine;

public interface IWaveSystemActions
{
    public event Action<uint> SpawnWaveEvent;
}
