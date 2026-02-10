using UnityEngine;

public interface IDamageSystem
{
    T GetDamageCalc<T>() where T : class;
}
