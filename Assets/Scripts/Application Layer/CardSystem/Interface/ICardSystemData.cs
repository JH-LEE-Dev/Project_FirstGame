using System.Collections.Generic;
using UnityEngine;

public interface ICardSystemData
{
    IReadOnlyList<CardDataInstance> deckCards { get; }
    IReadOnlyList<CardDataInstance> handCards { get; }
    IReadOnlyList<CardDataInstance> graveCards { get; }
}
