using DG.Tweening;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberSystem : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private ObjectPoolingSystem damagePool;
    [SerializeField] private DamageNumUnitValue unitValue;
    [Space]
    [SerializeField] private Color noramlDamageColor = Color.white;
    [SerializeField] private Color critDamageColor = Color.orange;


    public DamageNumUnitValue UnitValue { get { return unitValue; } }

    private UIView_HUD owner;

    public void Init(UIView_HUD _hud) => owner = _hud;
    public void ReleaseUnit(GameObject obj) => damagePool?.Pool?.Release(obj);

    public void SpawnBasicDamageNumber(float _damage, bool bCritical, Vector3 _startWorldPos)
    {
        GameObject performer = damagePool.Pool.Get();
        performer.SetActive(true);

        if (null == performer)
            return;

        DamageNumUnit script = performer.GetComponent<DamageNumUnit>();
        if (null == script)
            return;

        if (bCritical)
            script.OnDamageColor(critDamageColor);
        else
            script.OnDamageColor(noramlDamageColor);

        script.SetupUnitValue(UnitValue, this);
        script.BasicSpawnUnit(_damage, _startWorldPos);
    }

    // 구현 예정.
    private void SpawnSumMotionDamageNumber(float _damage, Vector3 _startWorldPos, Vector3 _targetPos)
    {
        GameObject performer = damagePool.Pool.Get();

        if (null == performer)
            return;

        DamageNumUnit script = performer.GetComponent<DamageNumUnit>();
        script?.SumMotionSpawnUnit(_damage, _startWorldPos, _targetPos);
    }

    [Button]
    private void BasicTestButton()
    {
        SpawnBasicDamageNumber(200, true, Vector3.zero);
    }
}
