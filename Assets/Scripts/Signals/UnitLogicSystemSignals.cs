using UnityEngine;

namespace UnitLogicSystemSignals
{
    public struct PlayerTurnFinishedEvent : IPulicSignal { }
    public struct EnemyIsDeadEvent : IPulicSignal
    {
        public Vector2 position;

        public EnemyIsDeadEvent(Vector2 _position)
        {
            position = _position;
        }
    }

    public struct PlayerTakeDamageEvent : IPulicSignal
    {
        public float damage;

        public PlayerTakeDamageEvent(float _damage)
        {
            damage = _damage;
        }
    }
}
