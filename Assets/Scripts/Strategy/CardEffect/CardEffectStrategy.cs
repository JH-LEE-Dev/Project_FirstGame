using UnityEngine;
using System.Threading.Tasks;

public abstract class CardEffectStrategy : ScriptableObject
{
    protected Unit unit;
    public abstract void Initialize(Unit unit);
}