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
    public GameSystemActionContextType contextType;
}

public struct CardDataControlSystemEventData
{
    public CardDataControlSystemEventType eventType;
    public GameSystemActionContextType contextType;
}

/// <summary>
/// Enums ----------------------------------------------
/// </summary>
public enum EffectApplyType
{
    StatusSystem,
    System,
    SlotSystem,
    ComplexSystem,
    SelectionSystem,
    DataControlSystem,
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
    AquaBurst,
    WaterFog,
    AirBust,
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
    HandCardsToGrave,
    DuplicateCardsToGrave,
    CardsToHand,
    ExtinctionCardsToDeck,
    MAX
}

public enum ComplexSystemActionType
{
    HandPileExistEffectExecute,
}


public enum CardLogicSystemEffectType
{
    FlareAdditionalDraw,
    SacrificeAdditionalDraw,
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
    Pluto,
    BattlePrep,
    BatteryCharge,
    NaturalCycle,
    Cleanse,
    ElementalBoost,
    WeaknessHarvest,
    OffenseReorder,
}

public enum CardSelectionSystemEffectType
{

}

public enum GameSystemActionTimingType
{
    BeforeAttack,
    AfterAttack,
    BeforeCardUsingPhase,
    Instant,
    BeforeTurn,
    AfterCardUsingPhase,
    GameStarted,
}

//AfterAttack에서는 반드시 StatusEffect만이 존재해야 함. 게임 로직으로도.
//BeforeAttack -> AfterAttack -> BeforeCardUsingPhase -> AfterCardUsingPhase -> Turn End -> Before Turn ... 

public enum CardDataControlSystemActionType
{
    CardsUpgraded,
    CardsPermenantlyUpgraded,
    RevertCardsUpgrade,
    MAX,
}


public enum CardEffectPriority
{
    Multiplier,
    Adder,
    totalDamageMultiplier,
    Normal,
}

public enum GameSystemActionContextType
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
    HandCardsToDeck,
    MAX,
    NoContext,
}