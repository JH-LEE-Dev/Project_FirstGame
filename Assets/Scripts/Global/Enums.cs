public enum UILayer
{
    Screen,     // 전체 화면 (인벤토리, 설정, 메인메뉴 등)
    Popup,      // 작은 팝업 (경고, 확인창 등)
    Overlay,    // HUD, 체력바, 미니맵 등
    Tooltip     // 툴팁, 마우스 오버 텍스트 등
}

public enum Dir
{ 
    Up,
    RightUp,
    Right,
    RightDown,
    Down,
}

public enum CardType
{
    Attack,
    Shield
}

public enum SceneType
{
    MainMenu,
    Gameplay
}

public enum PlayerTurnStateProcess
{
    CardDraw,
    CardUsing,
    Fire,
}

