using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Structs-----------------------------------------
/// </summary>
public struct CardUsedResult
{
    public bool bVerified;
    public int slotIdx;
    public CardDataInstance usedCard;
}

public struct BulletCardUsedResult
{
    public bool bVerified;
    public int slotIdx;
}

public struct CardListPriorityComparer : IComparer<List<CardDataInstance>>
{
    public int Compare(List<CardDataInstance> x, List<CardDataInstance> y)
    {
        bool xEmpty = x == null || x.Count == 0;
        bool yEmpty = y == null || y.Count == 0;

        if (xEmpty && yEmpty) return 0;
        if (xEmpty) return 1;
        if (yEmpty) return -1;

        int xPriority = (int)x[0].GetCardData().priority;
        int yPriority = (int)y[0].GetCardData().priority;

        return xPriority.CompareTo(yPriority);
    }
}

public struct CardIdComparer : IComparer<CardDataInstance>
{
    public int Compare(CardDataInstance x, CardDataInstance y)
    {
        return x.GetCardData().id.CompareTo(y.GetCardData().id);
    }
}

public struct CardLogicSystemEventData
{
    public CardLogicSystemEventType eventType;
    public CardSystemContextType contextType;
}

public struct CardDataControlSystemEventData
{
    public CardDataControlSystemEventType eventType;
    public CardSystemContextType contextType;
}

/// <summary>
/// Enums ----------------------------------------------
/// </summary>
public enum CardEffectApplyType
{
    StatusSystem,
    System,
    SlotSystem,
    ComplexSystem,
    SelectionSystem,
}


public enum CardStatusEffectType
{
    BonusDamage,
    Shield,
    BonusRange,
    FlareBonusDamage,
    HPDecrease,
    CriticalChance,
    RiftDetection,
    OvercompensationHPIncrease,
    OvercompensationShield,
    Distortion,
}

public enum CardLogicSystemActionType
{
    CardPileDraw,
    UsedCardsRemoveFromHand,
    UsedCardsToGrave,
    UsedCardsToExtinction,
    ResetCardPiles,
    SlotCardsToExtinction,
    SlotCardsToGrave,
    DuplicateCardsToDeck,
    DuplicateCardsToHand,
    GraveCardsToDeck,
    GraveCardsToHand,
    MAX
}

public enum CardLogicSystemEffectType
{
    FlareAdditionalDraw,
    SacrificeAdditionalDraw,
    Pluto,
}

public enum CardDataControlSystemEffectType
{

}

public enum CardSlotSystemEffectType
{
    SecureTheZone,
}

public enum ComplexSystemEffectType
{
    MeteorShower,
    SpaceShuttle,
    FinalOrbit,
    Recompense,
    HandEnhancement,
    QuantumEntanglement,
    HalleysComet,
    Scan,
    Amplify,
}

public enum CardSelectionSystemEffectType
{

}

public enum CardSystemActionTimingType
{
    BeforeAttack,
    AfterAttack,
    BeforeTurn,
    Instant,
}

public enum CardDataControlSystemActionType
{
    CardsUpgraded,
    MAX,
}


public enum CardEffectPriority
{
    Multiplier,
    Adder,
    Normal,
}

public enum CardSystemContextType
{
    CardPileDraw,
    UsedCardsRemoveFromHand,
    UsedCardsToGrave,
    UsedCardsToExtinction,
    ExtinctionCardsToDeck,
    SlotCardsToExtinction,
    SlotCardsToGrave,
    GraveCardsToHand,
    DuplicateCardCardsToDeck,
    DuplicateCardCardsToHand,
    GraveCardsToDeck,
    UpgradeCardsFromDeck,
    UpgradeCardsFromHand,
    UpgradeCardsFromExtinction,
    UpgradeCardsFromGrave,
    MAX
}