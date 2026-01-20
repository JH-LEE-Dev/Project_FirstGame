using UnityEngine;

namespace UnitLogicSystemSignals
{
    public struct PlayerAttackFinishedSignal  { }
    public struct EnemyIsDeadSignal 
    {
        public Vector2 position;

        public EnemyIsDeadSignal(Vector2 _position)
        {
            position = _position;
        }
    }

    public struct PlayerTakeDamageSignal 
    {
        public float damage;

        public PlayerTakeDamageSignal(float _damage)
        {
            damage = _damage;
        }
    }
}
