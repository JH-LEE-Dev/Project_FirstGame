
public static class SYSTEM_VAR
{
    public const int maxCardCount = 50;
    public const int maxDeckPileCount = 50;
    public const int limitDeckPileCount = 60;
    public const int maxHandPileCount = 12;
}

public enum CardType
{
    Bullet,
    Magic,
    Inherence,
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
    Recompense,
    QuantumEntanglement,
    HandEnhancement,
    HalleysComet,
    Scan,
    PrismBolt,
    ArcDischarge,
}
