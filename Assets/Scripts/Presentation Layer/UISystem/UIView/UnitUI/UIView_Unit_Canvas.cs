using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
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

    }

    private HealthBar_Enemy GetHealthBar()
    {
        GameObject obj = healthPool.Pool.Get();
        HealthBar_Enemy bar = obj?.GetComponent<HealthBar_Enemy>();
        if (null == bar)
            return null;

        obj.SetActive(true);

        return bar;
    }

    private void BindingEnemy(Enemy _target)
    {
        HealthBar_Enemy hpBar = GetHealthBar();
        if (null == hpBar)
            return;

        hpBar.Init(_target);
    }
}
