using System.Collections.Generic;
using UnityEngine;

public class UIView_Unit_Canvas : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    [Header("Pooling System")]
    [SerializeField] private ObjectPoolingSystem healthPool;

    ICharacterData characterData;
    IReadOnlyList<IEnemyData> enemyDatas;

    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection(ICharacterData _characterData, IReadOnlyList<IEnemyData> _enemyDatas)
    {
        characterData = _characterData;
        enemyDatas = _enemyDatas;
    }

    public override void Update()
    {
        base.Update();
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public void AdditionalEnemySpawned(IReadOnlyList<IEnemyData> _enemyDatas)
    {
        foreach (IEnemyData data in _enemyDatas)
        {
            Enemy script = data.GetTransform().GetComponent<Enemy>();
            if (null == script)
                continue;

            BindingEnemy(script);
        }
    }

    private EnemyUI GetEnemyUI()
    {
        GameObject obj = healthPool.Pool.Get();
        EnemyUI ui = obj?.GetComponent<EnemyUI>();
        if (null == ui)
            return null;

        return ui;
    }

    private void ReturnHealthBar(GameObject target)
    {
        if (!target.activeSelf)
            return;

        healthPool.Pool.Release(target);
    }

    private void BindingEnemy(Enemy _target)
    {
        EnemyUI ui = GetEnemyUI();
        if (null == ui)
            return;

        ui.gameObject.SetActive(true);
        ui.Init(_target, ReturnHealthBar);
    }

    public void WaveEnded()
    {

    }
}
