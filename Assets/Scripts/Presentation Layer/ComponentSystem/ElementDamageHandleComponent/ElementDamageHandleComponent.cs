using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementDamageHandleComponent
{
    //외부 의존성
    private IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> debuffs;

    public delegate float DamageCalcHandler(BulletElementType type, float _damage);
    private DamageCalcHandler[] damageCalcCreatorMap;

    public void Initialize(IReadOnlyDictionary<DebuffElementEffectType, DebuffElementData> _debuffs)
    {
        debuffs = _debuffs;

        damageCalcCreatorMap = new DamageCalcHandler[(int)BulletElementType.MAX];

        //Card Logic System 맵 할당
        BindLogic(BulletElementType.Electric, CalcElectricDamage);
        BindLogic(BulletElementType.Water, CalcWetDamage);

        void BindLogic(BulletElementType _type, DamageCalcHandler _action)
            => damageCalcCreatorMap[(int)_type] = _action;
    }

    public float GetResultDamage(IReadOnlyDictionary<BulletElementType, BulletElementData> _bulletElements, float _damage)
    {
        float resultDamage = _damage;

        foreach (KeyValuePair<BulletElementType, BulletElementData> pair in _bulletElements)
        {
            resultDamage += damageCalcCreatorMap[(int)pair.Key].Invoke(pair.Key, _damage);
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
}
