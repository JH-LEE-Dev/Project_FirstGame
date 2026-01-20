using UnityEngine;

namespace UnitLogicSystemSignals
{
    public struct PlayerAttackFinishedEvent  { }
    public struct EnemyIsDeadEvent 
    {
        public Vector2 position;

        public EnemyIsDeadEvent(Vector2 _position)
        {
            position = _position;
        }
    }

    public struct PlayerTakeDamageEvent 
    {
        public float damage;

        public PlayerTakeDamageEvent(float _damage)
        {
            damage = _damage;
        }
    }
}
