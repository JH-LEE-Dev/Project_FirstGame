using System.Collections.Generic;
using UnityEngine;
using System;

public class ElementExplosionSystem : MonoBehaviour
{
    public event Action<ElementExplosionType> ElementExplosionOccuredEvent; //원소 폭발 발생 시 Invoke

    [SerializeField] private List<ExplosionBehavior> explosionBehaviors = new List<ExplosionBehavior>((int)ElementExplosionType.MAX);

    private List<ElementExplosionType> explodedTypes = new List<ElementExplosionType>((int)ElementExplosionType.MAX);

    public void Initialize()
    {
        ExplosionComparer comparer = new ExplosionComparer();

        explosionBehaviors.Sort(comparer);
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
            explosionBehaviors[(int)explodedTypes[i]].Explode();
            ElementExplosionOccuredEvent?.Invoke(explodedTypes[i]);
        }

        explodedTypes.Clear();
    }
}
