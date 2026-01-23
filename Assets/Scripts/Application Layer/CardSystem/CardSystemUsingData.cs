
public static class SYSTEM_VAR
{
    public const int maxDeckPileCount = 30;
}

public enum CardType
{
    Bullet,
    Magic,
}

public enum ElementType
{
    Rotation, // ·ÎÅ×ÀÌ¼Ç
    Extinction, // ¼Ò¸ê
}

public enum UsingType
{
    Nesting, // ÁßÃ¸
    NotNesting,
}

//cardID¿Í È£È¯µÊ.
public enum CardName
{
    BonusDamage,
    Shield,
    BonusRange,
    Amplify,
    MeteorShower,
    Flare,
    Sacrifice,
    SpaceShuttle,
    FinalOrbit,
    SecureTheZone,
    CriticalHit,
    RiftDetection,
    Pluto,
    Overcompensation,
    Distortion,
}

public enum CardEffectApplyType
{
    Status,
    System,
    SlotSystem,
    ComplexSystem,
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

public enum CardSystemEffectType
{
    FlareAdditionalDraw,
    SacrificeAdditionalDraw,
    Pluto,
}

public enum CardSlotSystemEffectType
{
    Amplify,
    SecureTheZone,
}

public enum ComplexSystemEffectType
{
    MeteorShower,
    SpaceShuttle,
    FinalOrbit,
}

public enum CardSystemActionTimingType
{
    BeforeAttack,
    AfterAttack,
    BeforeTurn,
    Instant,
}

public enum CardSystemActionType
{
    CardPileDraw,
    CardToGrave,
    CardToExtinction,
}

public enum CardEffectPriority
{
    Multiplier,
    Adder,
    Normal,
}
