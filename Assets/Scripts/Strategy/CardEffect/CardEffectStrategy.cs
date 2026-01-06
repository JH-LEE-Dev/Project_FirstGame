using UnityEngine;
using System.Threading.Tasks;

public abstract class CardEffectStrategy : ScriptableObject
{
    protected Character character;
    public abstract void Initialize(Character character);

    public abstract void Execute();
}