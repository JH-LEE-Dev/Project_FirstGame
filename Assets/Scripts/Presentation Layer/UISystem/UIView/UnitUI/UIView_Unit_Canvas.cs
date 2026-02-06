using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class UIView_Unit_Canvas : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;

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
}
