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
    Selecting,
    Hidden
}

public enum CardReturnType
{
    FlyToGrave,
    Extinction,
    EquippedAction,
    StayHand,
    Temp,
}

public enum CurrentPannel
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
