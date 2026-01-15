using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public interface IUnitLogicSystemActions
{
    void DependencyInjection_Character(Character character);
    void DependencyInjection_Earth(Earth earth);
    void DependencyInjection_Enemy(List<Enemy> enemies);

    bool CanApplyBulletEffect();
}
