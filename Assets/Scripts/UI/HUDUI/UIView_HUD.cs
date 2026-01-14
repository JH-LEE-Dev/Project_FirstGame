using DamageNumbersPro;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIView_HUD : UIView
{
    //외부 데이터
    IPlayerData playerData;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;
    [Space]
    [SerializeField] private TMP_Text turnIndicatorText;
    [SerializeField] private TMP_Text turnProcessIndicatorText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text waveEndDeclareText;

    [Header("UI Bar")]
    [SerializeField] private BarMotion hpBar;
    [SerializeField] private UIText_PlayerHP hpText;
    [SerializeField] private BarMotion targetBar;

    [Header("Pooling System")]
    [SerializeField] private UIDamage_Pooling damagePooling;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);
    }

    private void Start()
    {
        waveEndDeclareText.gameObject.SetActive(false);
        hpText?.Init(playerData.GetMaxHealth(), this);
    }

    public void DataInjection(IPlayerData _playerData)
    {
        playerData = _playerData;
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
        waveEndDeclareText?.gameObject.SetActive(false); 
        waveText.text = "Wave : " + (waveIdx + 1).ToString();
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
        if (null == hpBar)
            return;

        IPlayerData currPlayer = playerData;

        float maxHp = currPlayer.GetMaxHealth();
        float prevHp = currPlayer.GetPrevHealth();
        float currHp = currPlayer.GetCurrentHealth();

        float oneProgress = currHp / maxHp;

        if (null != hpBar)
            hpBar.OnHit(oneProgress);

        if (null != hpText)
            hpText.OnHit(prevHp, currHp, oneProgress, damage, _damagNum: damagePooling.DamagePool.Get());
    }

    public void ReturnDamageText(GameObject target) => damagePooling?.DamagePool.Release(target);
    public GameObject GetDamageObj() => damagePooling.DamagePool.Get();

    public void PlayerSpawned()
    {

    }
}
