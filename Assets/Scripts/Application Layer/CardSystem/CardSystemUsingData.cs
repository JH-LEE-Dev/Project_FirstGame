
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
}

public enum CardSystemEffectType
{
    FlareAdditionalDraw,
    SacrificeAdditionalDraw,
}

public enum CardSlotSystemEffectType
{
    Amplify,
}

public enum ComplexSystemEffectType
{
    MeteorShower,
    SpaceShuttle,
}

public enum CardSystemActionTimingType
{
    BeforeAttack,
    AfterAttack,
    BeforeTurn,
}

public enum CardEffectPriority
{
    Multiplier,
    Adder,
    Normal,
}
