using UnityEngine;

public enum UILayer
{
    Screen,     // 전체 화면 (인벤토리, 설정, 메인메뉴 등)
    Popup,      // 작은 팝업 (경고, 확인창 등)
    Overlay,    // HUD, 체력바, 미니맵 등
    Tooltip,     // 툴팁, 마우스 오버 텍스트 등
    World       // 월드 UI
}

public enum Dir
{ 
    Right,
    Left,
}

public enum SceneType
{
    MainMenu,
    Gameplay
}

public enum CardInstanceType
{
    Hand,
    Other,
    Shop,
    END
}

public enum CardState
{
    InHand,
    Preview,
    Equipped,
    Other,
    EffectInHand,
    EffectOther,
    Selecting,
    Hidden
}

public enum CardReturnType
{
    FlyToGrave,     // 사용없는 연출. 묘지로 슝 날아가여 풀링반납.

    Extinction,     // 소멸 연출 이후 풀링반납.

    MagicUse,       // 마법 연출 이후 풀링반납. (묘지로 가는 별똥별)

    ToDeck,         // 별이 되어서 덱으로

    StayHand,       // 손에 그대로 있기. 풀링반납안함. 다시 패로 감.

    Temp,           // 즉시 삭제
}

public enum CardZone
{
    Deck,
    Grave,
    Extinction,
    NONE
}

public enum CombatActionSignal
{
    Attack,
}

public enum MoveActionSignal
{
    Idle,
    LeftMoving,
    RightMoving,
    LeftBlocked,
    RightBlocked,
    NotBlocked,
}

public enum CutsceneSignal
{
    TurnStart_Start,
    TurnStart_End,
    TurnEnd_Start,
    TurnEnd_End,
}

enum ShowOption
{
    OnEnter,
    OnExit,
    OnUp,
    OnDown,
};

enum RectSelect
{
    Top,
    Middle,
    Bottom,
};

public enum StarLightAcquisitionType
{
    Kill,
    Ability,
    OverKill,
}

public enum ShopCardState
{ 
    Idle,
    Select,
}

public enum PlayerStatType
{
    AttackCount,
    AttackRange,
    CriticalChance,
    AttackDamage,
    AdditionalDamage,
    WeaknessTurnCount,
    BulletEffectElemental,
    BulletDebuff,
}

public enum CharacterType
{
    Rumy,
}
