using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIView_HUD : UIView
{
    //외부 데이터
    IPlayerData playerData;
    IWaveSystemData waveSystemData;

    [Header("UI References")]
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameObject uiPrefab;
    [Space]
    [SerializeField] private TMP_Text turnIndicatorText;
    [SerializeField] private TMP_Text turnProcessIndicatorText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text waveStateDeclareText;

    [Header("UI Bar")]
    [SerializeField] private BarMotion hpBar;
    [SerializeField] private UIText_PlayerHP hpText;
    [SerializeField] private BarMotion targetBar;
    [SerializeField] private UIText_TargetGage targetGageText;

    [Header("Pooling System")]
    [SerializeField] private ObjectPoolingSystem damagePool;
    [SerializeField] private ObjectPoolingSystem targetBarEffectPool;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);
    }

    public void PlayerSpawned(IPlayerData _playerData)
    {
        playerData = _playerData;

        IntializeChildrenHUD();
    }

    private void Start()
    {
        
    }

    public void DataInjection(IWaveSystemData _waveSystemData)
    {
        waveSystemData = _waveSystemData;
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

    public void WaveStarted(int _waveIdx)
    {
        waveStateDeclareText.text = "WaveStarted!!";
        waveText.text = "Wave : " + (_waveIdx + 1).ToString();
        turnIndicatorText.text = "WaveStarted";
        turnProcessIndicatorText.text = "Prepare For Wave";

        IntializeChildrenHUD();
    }

    public void GameStarted()
    {
        waveStateDeclareText.gameObject.SetActive(true);
        waveStateDeclareText.text = "GameStarted!!!";
    }

    public void WaveEnded()
    {
        waveStateDeclareText.gameObject.SetActive(true);
        waveStateDeclareText.text = "Prepare For Next Wave!!";
    }

    public void PlayerTurnStarted()
    {
        waveStateDeclareText?.gameObject.SetActive(false); 
        waveText.text = "Wave : " + (0 + 1).ToString();
        turnIndicatorText.text = "PlayerTurn";
        turnProcessIndicatorText.text = "Card Draw";
    }

    public void EnemyTurnStarted()
    {
        turnIndicatorText.text = "EnemyTurn";
        turnProcessIndicatorText.text = "Enemy Moving!";
    }

    public void EnemyIsDead(Vector2 deadPosition)
    {
        Target_BarUpdate(deadPosition);
    }

    public void CardUseTimeStarted()
    {
        turnProcessIndicatorText.text = "Card Using Time";
    }

    public void OnPlayerHit(float damage)
    {
        HP_BarUpdate(damage);
    }

    private void HP_BarUpdate(float damage)
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
            hpText.OnHit(prevHp, currHp, oneProgress, damage, _damagNum: damagePool.Pool.Get());
    }

    private void Target_BarUpdate(Vector2 worldDeadPos)
    {
        if (null == targetBar)
            return;

        GameObject vfx = targetBarEffectPool?.Pool.Get();
        if (null == vfx)
            return;

        RectTransform vfxRect = vfx.GetComponent<RectTransform>();
        if (null == vfxRect)
            return;

        VFX_TargetBarStar script = vfx.GetComponent<VFX_TargetBarStar>();
        if (null == script)
            return;

        vfx.SetActive(true);
        vfxRect.anchoredPosition = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(worldDeadPos, vfxRect);

        int maxEnemyCnt = waveSystemData.GetMaxWaveProgress();
        int currentEnemyCnt = waveSystemData.GetCurrentWaveProgress() - 1;
        float currentKillCnt = maxEnemyCnt - currentEnemyCnt;

        float currentProgress = currentKillCnt / maxEnemyCnt;

        Action callback = () =>
        {
            targetBar.OnFill(currentProgress);
            targetGageText?.DataUpdate(currentKillCnt, maxEnemyCnt);
            StartCoroutine(ReleaseEffect(script));
        };

        script.Play(targetBar.GetAnchoredPos(), callback);
    }

    private IEnumerator ReleaseEffect(VFX_TargetBarStar target)
    {
        while (target.CheckAliveParticle())
            yield return new WaitForSeconds(0.2f);

        targetBarEffectPool.Pool.Release(target.gameObject);
    }

    private void IntializeChildrenHUD()
    {
        hpText?.Init(playerData.GetMaxHealth(), this);
        hpBar?.Init(playerData.GetCurrentHealth() / playerData.GetMaxHealth());
        targetBar?.Init(0f, waveSystemData.GetMaxWaveProgress());
        targetGageText?.DataUpdate(0f, waveSystemData.GetMaxWaveProgress());
    }

    public void ReturnDamageText(GameObject target) => damagePool?.Pool.Release(target);
    public GameObject GetDamageObj() => damagePool.Pool.Get();
}
