using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface IUnitLogicSystemActions
{
    void Initialize(Character character,Earth earth, List<Enemy> enemies);

    bool CanApplyBulletEffect();
}
