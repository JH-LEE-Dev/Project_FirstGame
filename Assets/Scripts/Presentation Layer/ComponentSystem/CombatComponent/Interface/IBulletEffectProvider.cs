using System.Collections.Generic;
using UnityEngine;

public interface IBulletEffectProvider
{
    BulletType bulletType { get; }
    bool bUpgraded { get; }
    IReadOnlyDictionary<BulletElementType, BulletElementData> currentEffectElements {  get; }   
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> currentDebuffElementTypes { get; }
}
