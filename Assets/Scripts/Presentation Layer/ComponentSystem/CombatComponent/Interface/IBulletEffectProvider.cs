using System.Collections.Generic;
using UnityEngine;

public interface IBulletEffectProvider
{
    BulletType bulletType { get; }
    bool bUpgraded { get; }
    List<BulletElementData> currentBulletElementTypes { get; }
    List<DebuffElementData> currentDebuffElementTypes { get; }
}
