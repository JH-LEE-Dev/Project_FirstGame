using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIView_HUD : UIView
{
    //외부 의존성
    IUnitLogicSystemProvider unitLogicSystemProvider;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;
    [Space]
    [SerializeField] private TMP_Text turnIndicatorText;
    [SerializeField] private TMP_Text turnProcessIndicatorText;

    [Header("UI Bar")]
    [SerializeField] private BarMotion hpBar;
    [SerializeField] private TextMotion hpText;
    [SerializeField] private BarMotion targetBar;

    private float prevCurrHp = 0f;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);
    }

    public void DependencyInjection(IUnitLogicSystemProvider _unitLogicSystemProvider)
    {
        unitLogicSystemProvider = _unitLogicSystemProvider;
    }

    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    private void OnClickClose()
    {
    }

    public void RenderUI()
    {

    }

    public void PlayerTurnStarted(int waveIdx)
    {
        turnIndicatorText.text = "PlayerTurn";
        turnProcessIndicatorText.text = "Card Draw";
    }

    public void EnemyTurnStarted()
    {
        turnIndicatorText.text = "EnemyTurn";
        turnProcessIndicatorText.text = "Enemy Moving!";
    }

    public void CardUseTimeStarted()
    {
        turnProcessIndicatorText.text = "Card Using Time";
    }

    public void OnPlayerHit(float damage)
    {
        IEarthData currEarth = unitLogicSystemProvider.earthData;

        float maxHP = currEarth.GetMaxHealth();
        float currHp = currEarth.GetCurrentHealth();

        float oneProgress = currHp / maxHP;

        if (null != hpBar)
            hpBar.OnHit(oneProgress);

        if (null != hpText)
            hpText.OnHit(prevCurrHp, currHp);

        prevCurrHp = currHp;
    }
}
