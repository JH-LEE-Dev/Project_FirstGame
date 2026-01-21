
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
}

public enum CardEffectApplyType
{
    Status,
    System,
    SlotSystem
}


public enum CardStatusEffectType
{
    BonusDamage,
    Shield,
    BonusRange,
}

public enum CardSystemEffectType
{

}

public enum CardSlotSystemEffectType
{
    Amplify,
}

public enum CardSystemActionTimingType
{
    BeforeAttack,
    AfterAttack,
    NextTurn,
}

public enum CardEffectPriority
{
    Multiplier,
    Adder,
    Normal,
}
