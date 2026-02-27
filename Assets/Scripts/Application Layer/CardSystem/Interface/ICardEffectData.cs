using System.Collections.Generic;

public interface ICardEffectData
{
    bool bUpgraded { get; }
    IReadOnlyDictionary<BulletElementType, BulletElementData> elementTypes { get; }
    IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffTypes { get; }
    IReadOnlyDictionary<EffectModType, EffectModData> effectModifiers { get; }
}
