
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
    Nesting // ÁßÃ¸
}

//cardID¿Í È£È¯µÊ.
public enum CardName
{
    BonusDamage,
    DrawAgain,
    MeteorShower,
    Flare,
    Shield
}

public enum CardEffectApplyType
{
    Status,
    System
}


public enum CardStatusEffectType
{
    BonusDamage,
    Shield
}

public enum CardSystemEffectType
{
    DrawAgain,
    MeteorShower,
    Flare,
    AttackAgain
}

public enum CardSystemActionTimingType
{
    BeforeAttack,
    AfterAttack,
    NextTurn,
}