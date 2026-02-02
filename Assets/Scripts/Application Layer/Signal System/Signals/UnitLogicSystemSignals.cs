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
    public struct PlayerGetShieldSignal
    {
        public float amount;

        public PlayerGetShieldSignal(float _amount)
        {
            amount = _amount;
        }
    }
    public struct PlayerGetHPSignal
    {
        public float amount;

        public PlayerGetHPSignal(float _amount)
        {
            amount = _amount;
        }
    }
    public struct PlayerAttackedSignal { }
    public struct EnemyTakeDamageSignal 
    {
        public IEnemyData enemyData;
        public float damage;
        public bool bCritical;
        public EnemyTakeDamageSignal(IEnemyData _enemyData,float _damage,bool _bCritical)
        {
            enemyData = _enemyData;
            damage = _damage;
            bCritical = _bCritical;
        }
    }
    public struct EnemyIsKilledSignal 
    {
        public IEnemyData enemyData;
        public EnemyIsKilledSignal(IEnemyData _enemyData)
        {
            enemyData = _enemyData;
        }
    }
    public struct WaveProgressUpdatedSignal
    {
        public IEnemyData enemyData;
        public WaveProgressUpdatedSignal(IEnemyData _enemyData)
        {
            enemyData = _enemyData;
        }
    }
    public struct ResetPlayerShieldSignal { }
    public struct CharacterStatChangedSignal { }
}
