using System;

public static class SYSTEM_VAR
{
    public const int maxCardCount = 50;
    public const int maxDeckPileCount = 50;
    public const int limitDeckPileCount = 60;
    public const int maxHandPileCount = 12;

    public const int maxArtifactCount = 5;

    public const int maxDebuffElementCount = (int)BulletElementType.MAX;

    public const int maxEnemyCount = 50;
}

public enum CardType
{
    Bullet,
    Magic,
    Inherence,
    Debuff,
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
    AquaBurst,
    WaterFog,
    BattlePrep,
    BatteryCharge,
    NaturalCycle,
    Cleanse,
    ElementalBoost,
    AirBust,
}

public enum BulletElementType
{
    Normal,
    Electric,
    Water,
    Fire,
    Poison,
    MAX,
}

public enum BulletType
{
    Normal,
    PrismBolt,
    ArcDischarge,
    AquaBurst,
}

[Serializable]
public struct BulletElementData
{
    public BulletElementType bulletElementType;
    public int nestingCnt;

    public BulletElementData(BulletElementType _effectElementType, int _nestingCnt)
    {
        bulletElementType = _effectElementType;
        nestingCnt = _nestingCnt;
    }
}

[Serializable]
public struct DebuffElementData
{
    public DebuffElementEffectType debuffElementType;
    public int turnCnt;

    public DebuffElementData(DebuffElementEffectType _effectElementType,int _turnCnt)
    {
        debuffElementType = _effectElementType;
        turnCnt = _turnCnt;
    }
}

public enum DebuffElementEffectType
{
    Combustion, //¿¬¼Ò
    ElectricShock, //°¨Àü
    Oxidation, //»êÈ­
    Wet, //½ÀÀ± ¤»
}

public enum ElementExplosionType
{
    Steam,
    Flame,
    Spark,
}

public struct AdditionalAttackStat
{
    public float attack;
    public float additionalAttackValue;
    public float totalDamageValue;
    public AdditionalAttackStat(float _attack, float _additionalAttackValue, float _totalDamageValue)
    {
        attack = _attack;
        additionalAttackValue = _additionalAttackValue;
        totalDamageValue = _totalDamageValue;
    }
}

