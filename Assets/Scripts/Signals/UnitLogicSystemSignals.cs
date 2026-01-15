using UnityEngine;

namespace UnitLogicSystemSignals
{
    public struct PlayerTurnFinishedEvent { }
    public struct EnemyIsDeadEvent { }

    public struct PlayerTakeDamageEvent
    {
        public float damage;

        public PlayerTakeDamageEvent(float _damage)
        {
            damage = _damage;
        }
    }
}
