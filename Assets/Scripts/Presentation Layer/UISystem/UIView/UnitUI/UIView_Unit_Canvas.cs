using UnityEngine;
using System;

public class UIView_Unit_Canvas : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

    [Header("Pooling System")]
    [SerializeField] private ObjectPoolingSystem healthPool;

    protected override void Awake()
    {
        base.Awake();
    }

    public void DataInjection()
    {

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

    private HPBar_Enemy GetHealthBar()
    {
        GameObject obj = healthPool.Pool.Get();
        HPBar_Enemy bar = obj?.GetComponent<HPBar_Enemy>();
        if (null == bar)
            return null;

        obj.SetActive(true);

        return bar;
    }

    private void BindingEnemy(Enemy _target)
    {
        HPBar_Enemy hpBar = GetHealthBar();
        if (null == hpBar)
            return;

        hpBar.Init(_target);
    }
}
