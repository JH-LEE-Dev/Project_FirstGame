
public enum CardType
{
    Bullet,
    Magic,
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