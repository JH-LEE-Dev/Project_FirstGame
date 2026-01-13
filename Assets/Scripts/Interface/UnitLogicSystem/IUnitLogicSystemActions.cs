using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface IUnitLogicSystemActions
{
    void Initialize(Character character);
    void Initialize(Earth earth);
    void Initialize(List<Enemy> enemies);

    bool CanApplyBulletEffect();
}
