using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;

public class ElementExplosionSystem : MonoBehaviour
{
    public event Action<ElementExplosionType> ElementExplosionOccuredEvent; //원소 폭발 발생 시 Invoke

    [SerializeField] private List<Explosion> explosions = new List<Explosion>((int)ElementExplosionType.MAX);

    private List<ElementExplosionType> explodedTypes = new List<ElementExplosionType>((int)ElementExplosionType.MAX);

    private Dictionary<ElementExplosionType, ObjectPool<Explosion>> explosionPools
= new Dictionary<ElementExplosionType, ObjectPool<Explosion>>();

    public delegate void ExplosionHandler(Collider2D[] _colliders);
    private ExplosionHandler[] explosionHandlerCreator;

    public const int steamDamage = 20;
    public const int flameDamage = 15;
    public const int sparkDamage = 30;

    public void Initialize()
    {
        ExplosionComparer comparer = new ExplosionComparer();

        if (explosions.Count == 0)
            return;

        explosions.Sort(comparer);

        for (int i = 0; i < explosions.Count; ++i)
        {
            ObjectPool<Explosion> pool = new ObjectPool<Explosion>(
                createFunc: () =>
                {
                    Explosion instance = Instantiate(explosions[i]);

                    return instance;
                },
                actionOnGet: explosion =>
                {
                    explosion.ExplosionEndEvent -= ExplosionEnd;
                    explosion.ExplosionEndEvent += ExplosionEnd;

                    explosion.ApplyExplosionEvent -= HandleExplosion;
                    explosion.ApplyExplosionEvent += HandleExplosion;
                },
                actionOnRelease: explosion =>
                {
                    explosion.ExplosionEndEvent -= ExplosionEnd;

                    explosion.ApplyExplosionEvent -= HandleExplosion;
                },
                actionOnDestroy: null,
                collectionCheck: false,
                defaultCapacity: SYSTEM_VAR.maxExplosionCount,
                maxSize: SYSTEM_VAR.maxExplosionCount
            );

            explosionPools.Add(explosions[i].elementExplosionType, pool);
        }


        explosionHandlerCreator = new ExplosionHandler[(int)ElementExplosionType.MAX];

        BindLogic(ElementExplosionType.Steam, HandleSteamExplosion);
        BindLogic(ElementExplosionType.Spark, HandleSparkExplosion);
        BindLogic(ElementExplosionType.Flame, HandleFlameExplosion);

        void BindLogic(ElementExplosionType type, ExplosionHandler action)
            => explosionHandlerCreator[(int)type] = action;
    }

    public void EnemyCollide(IEnemyData _enemy1, IEnemyData _enemy2)
    {
        if (_enemy1 == null || _enemy2 == null)
            return;

        if (_enemy1.enemyID > _enemy2.enemyID)
            return;

        EvaluateExplosionType(_enemy1.currentAppliedDebuff, _enemy2.currentAppliedDebuff);
    }

    public void EnemyHit(IEnemyData _data, IReadOnlyDictionary<BulletElementType, BulletElementData> _elements)
    {
        EvaluateExplosionType(_elements, _data.currentAppliedDebuff);
    }

    public void PlayerCollide(IPlayerData _data, IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _elements)
    {
        EvaluateExplosionType(_elements, _data.currentAppliedDebuff);
    }

    private void EvaluateExplosionType(IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements,
        IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements)
    {
        if (_bulletElements == null || _debuffElements == null)
            return;

        foreach (KeyValuePair<BulletElementType, BulletElementData> pair in _bulletElements)
        {
            if (pair.Key == BulletElementType.Electric)
            {
                if (_debuffElements.ContainsKey(DebuffElementEffectType.Wet))
                {
                    explodedTypes.Add(ElementExplosionType.Spark);
                }
            }

            if (pair.Key == BulletElementType.Fire)
            {
                if (_debuffElements.ContainsKey(DebuffElementEffectType.Oxidation))
                {
                    explodedTypes.Add(ElementExplosionType.Flame);
                }

                if (_debuffElements.ContainsKey(DebuffElementEffectType.Wet))
                {
                    explodedTypes.Add(ElementExplosionType.Steam);
                }
            }

            if (pair.Key == BulletElementType.Water)
            {
                if (_debuffElements.ContainsKey(DebuffElementEffectType.ElectricShock))
                {
                    explodedTypes.Add(ElementExplosionType.Spark);
                }

                if (_debuffElements.ContainsKey(DebuffElementEffectType.Combustion))
                {
                    explodedTypes.Add(ElementExplosionType.Steam);
                }
            }

            if (pair.Key == BulletElementType.Poison)
            {
                if (_debuffElements.ContainsKey(DebuffElementEffectType.Combustion))
                {
                    explodedTypes.Add(ElementExplosionType.Flame);
                }
            }
        }

        ExecuteExplosion();
    }

    private void EvaluateExplosionType(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements1,
       IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffElements2)
    {
        if (_debuffElements1 == null || _debuffElements2 == null)
            return;

        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in _debuffElements1)
        {
            if (pair.Key == DebuffElementEffectType.ElectricShock)
            {
                if (_debuffElements2.ContainsKey(DebuffElementEffectType.Wet))
                {
                    explodedTypes.Add(ElementExplosionType.Spark);
                }
            }

            if (pair.Key == DebuffElementEffectType.Combustion)
            {
                if (_debuffElements2.ContainsKey(DebuffElementEffectType.Oxidation))
                {
                    explodedTypes.Add(ElementExplosionType.Flame);
                }

                if (_debuffElements2.ContainsKey(DebuffElementEffectType.Wet))
                {
                    explodedTypes.Add(ElementExplosionType.Steam);
                }
            }

            if (pair.Key == DebuffElementEffectType.Wet)
            {
                if (_debuffElements2.ContainsKey(DebuffElementEffectType.ElectricShock))
                {
                    explodedTypes.Add(ElementExplosionType.Spark);
                }

                if (_debuffElements2.ContainsKey(DebuffElementEffectType.Combustion))
                {
                    explodedTypes.Add(ElementExplosionType.Steam);
                }
            }

            if (pair.Key == DebuffElementEffectType.Oxidation)
            {
                if (_debuffElements2.ContainsKey(DebuffElementEffectType.Combustion))
                {
                    explodedTypes.Add(ElementExplosionType.Flame);
                }
            }
        }

        ExecuteExplosion();
    }

    private void ExecuteExplosion()
    {
        for (int i = 0; i < explodedTypes.Count; ++i)
        {
            explosionPools[explodedTypes[i]].Get().Explode();
            ElementExplosionOccuredEvent?.Invoke(explodedTypes[i]);
        }

        explodedTypes.Clear();
    }

    private void ExplosionEnd(Explosion _explosion)
    {
        explosionPools[_explosion.elementExplosionType].Release(_explosion);
    }

    private void HandleExplosion(ElementExplosionType _type, Collider2D[] _colliders)
    {
        if (explosionHandlerCreator[(int)_type] != null)
            explosionHandlerCreator[(int)_type].Invoke(_colliders);
    }

    private void HandleSteamExplosion(Collider2D[] _colliders)
    {
        for (int i = 0; i < _colliders.Length; ++i)
        {
            var enemy = (IEnemyHandler)_colliders[i];

            if (enemy != null)
            {
                enemy.TakeDamage(steamDamage, false);
            }
        }
    }

    private void HandleSparkExplosion(Collider2D[] _colliders)
    {
        for (int i = 0; i < _colliders.Length; ++i)
        {
            var enemy = (IEnemyHandler)_colliders[i];

            if (enemy != null)
            {
                enemy.TakeDamage(sparkDamage, false);

                DebuffElementData debuffElementData = new DebuffElementData(DebuffElementEffectType.ElectricShock, 2);
                enemy.ApplyElementDebuff(debuffElementData);
            }
        }
    }

    private void HandleFlameExplosion(Collider2D[] _colliders)
    {
        for (int i = 0; i < _colliders.Length; ++i)
        {
            var enemy = (IEnemyHandler)_colliders[i];

            if (enemy != null)
            {
                // Bullet
                enemy.TakeDamage(flameDamage, false);

                DebuffElementData debuffElementData = new DebuffElementData(DebuffElementEffectType.Combustion, 2);
                enemy.ApplyElementDebuff(debuffElementData);
            }
        }
    }
}
