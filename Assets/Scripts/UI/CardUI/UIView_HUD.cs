using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_HUD : UIView
{
    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;
    [Space]
    [SerializeField] private TMP_Text turnIndicatorText;
    [SerializeField] private TMP_Text turnProcessIndicatorText;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);
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
}
