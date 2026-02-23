using System;

/// <summary>
/// Structs -----------------------------
/// </summary>

[Serializable]
public struct BulletBehaviorData
{
    public BulletType bulletType;
    public BulletBehavior behavior_BeforeFire;
    public BulletBehavior behavior_Fly;
    public BulletBehavior behavior_Hit;
}
