using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementDamageHandleComponent
{
    //외부 의존성
    private IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs;

    public delegate float BulletDamageCalcHandler(BulletElementType type, float _damage);
    private BulletDamageCalcHandler[] bulletDamageCalcCreatorMap;
    public delegate float CollideDamageCalcHandler(DebuffElementEffectType type, float _damage);
    private CollideDamageCalcHandler[] collideDamageCalcCreatorMap;

    public void Initialize(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffs)
    {
        debuffs = _debuffs;

        bulletDamageCalcCreatorMap = new BulletDamageCalcHandler[(int)BulletElementType.MAX];

        //Card Logic System 맵 할당
        BindLogic(BulletElementType.Electric, CalcElectricDamage);
        BindLogic(BulletElementType.Water, CalcWetDamage);

        void BindLogic(BulletElementType _type, BulletDamageCalcHandler _action)
            => bulletDamageCalcCreatorMap[(int)_type] = _action;

        collideDamageCalcCreatorMap = new CollideDamageCalcHandler[(int)DebuffElementEffectType.MAX];

        //Card Logic System 맵 할당
        BindCollideLogic(DebuffElementEffectType.ElectricShock, CalcElectricDamage);
        BindCollideLogic(DebuffElementEffectType.Wet, CalcWetDamage);

        void BindCollideLogic(DebuffElementEffectType _type, CollideDamageCalcHandler _action)
            => collideDamageCalcCreatorMap[(int)_type] = _action;
    }

    public float GetResultDamage(IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements, float _damage)
    {
        float resultDamage = _damage;

        if (_bulletElements == null)
            return resultDamage;

        foreach (KeyValuePair<BulletElementType, BulletElementData> pair in _bulletElements)
        {
            resultDamage += bulletDamageCalcCreatorMap[(int)pair.Key].Invoke(pair.Key, _damage);
        }

        return resultDamage;
    }

    public float GetResultDamage(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffs, float _damage)
    {
        float resultDamage = _damage;

        if (_debuffs == null)
            return resultDamage;

        foreach (KeyValuePair<DebuffElementEffectType, DebuffElementData> pair in _debuffs)
        {
            resultDamage += collideDamageCalcCreatorMap[(int)pair.Key].Invoke(pair.Key, _damage);
        }

        return resultDamage;
    }

    private float CalcElectricDamage(BulletElementType _type, float _damage)
    {
        if (debuffs.ContainsKey(DebuffElementEffectType.ElectricShock))
            _damage += _damage * 0.5f;

        return _damage;
    }

    private float CalcWetDamage(BulletElementType _type, float _damage)
    {
        if (debuffs.ContainsKey(DebuffElementEffectType.Wet))
            _damage += _damage * 0.5f;

        return _damage;
    }

    private float CalcElectricDamage(DebuffElementEffectType _type, float _damage)
    {
        if (debuffs.ContainsKey(DebuffElementEffectType.ElectricShock))
            _damage += _damage * 0.5f;

        return _damage;
    }

    private float CalcWetDamage(DebuffElementEffectType _type, float _damage)
    {
        if (debuffs.ContainsKey(DebuffElementEffectType.Wet))
            _damage += _damage * 0.5f;

        return _damage;
    }
}
