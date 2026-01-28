using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberSystem : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private GameObject damageUnitPrefab;
    [SerializeField] private DamageNumUnitValue unitValue;
    [Space]
    [SerializeField] private Color noramlDamageColor = Color.white;
    [SerializeField] private Color critDamageColor = Color.orange;

    public DamageNumUnitValue UnitValue { get { return unitValue; } }


    private UIView_HUD owner;

    public void Init(UIView_HUD _hud) => owner = _hud;

    private void Awake()
    {
        
    }

    public void SpawnBasicDamageNumber(GameObject _performer, float _damage, Vector3 _startWorldPos)
    {
        if (null == _performer)
            return;

        DamageNumUnit script = _performer.GetComponent<DamageNumUnit>();
        script?.BasicSpawnUnit(_damage, _startWorldPos);
    }

    public void SpawnSumMotionDamageNumber(GameObject _performer, float _damage, Vector3 _startWorldPos, Vector3 _targetPos)
    {
        if (null == _performer)
            return;

        DamageNumUnit script = _performer.GetComponent<DamageNumUnit>();
        script?.SumMotionSpawnUnit(_damage, _startWorldPos, _targetPos);
    }
}
