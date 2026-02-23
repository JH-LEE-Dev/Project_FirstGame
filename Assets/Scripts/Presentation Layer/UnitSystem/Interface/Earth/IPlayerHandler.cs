using System.Collections.Generic;
using UnityEngine;

public interface IPlayerHandler
{
    IPlayerData playerData { get; }
    void ClearDebuff();
    void ReleaseDebuff(DebuffElementData debuffElementData);
    void ReleaseDebuff(DebuffElementEffectType type);
    void TakeCollideDamage(float damage, bool bCritical, Vector2 pos, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements = null);
    void ApplyElementDebuff(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs, Vector2 pos = default);
    void ApplyElementDebuff(DebuffElementData debuff, Vector2 pos = default);
}
