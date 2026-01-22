using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

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

    private bool WaveStartFirstTime = true;

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

        WaveStartFirstTime = true;
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

        IntializeChildrenHUD();
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

    public void PlayerGetShield(float amount)
    {
        if (null == hpBar)
            return;

        float maxHp = playerData.GetMaxHealth();
        float currHp = playerData.GetCurrentHealth();
        float currShield = playerData.GetCurrentShield();

        float total = (currShield + currHp);

        float shieldProgress = total / maxHp;

        if (1f <= shieldProgress)
        {
            float hpRatio = currHp / total;

            hpBar.DirectShieldSet(1f);
            hpBar.CalcMain(hpRatio);
        }
        else
        {
            hpBar.CalcShield(Mathf.Clamp(shieldProgress, 0f, 1f));
        }
    }

    public void CardUseTimeStarted()
    {
        turnProcessIndicatorText.text = "Card Using Time";
    }

    public void OnPlayerHit(float damage)
    {
        HP_BarUpdateforDamaged(damage);
    }

    private void HP_BarUpdateforDamaged(float damage)
    {
        if (null == hpBar)
            return;

        float maxHp = playerData.GetMaxHealth();
        float prevHp = playerData.GetPrevHealth();
        float currHp = playerData.GetCurrentHealth();
        float currShield = playerData.GetCurrentShield();
        float prevShield = playerData.GetPrevShield();

        float prevTotalProgress = (prevShield + prevHp) / maxHp;
        float shieldProgress = Mathf.Clamp((currShield + currHp) / maxHp, 0f, 1f);
        float hpProgress = Mathf.Clamp(currHp / maxHp, 0f, 1f);

        // 맞기전 실드 + 체력 가중치가 1보다 작으면
        if (1f > prevTotalProgress && 0f < prevShield)
        {
            Action latePlay = () =>
            {
                hpBar.OnHit(hpProgress);
            };

            // 피해를 받고 쉴드가 남아있으면, 기존 체력 + 쉴드 적용 가중치를 적용
            // 피해를 받고 쉴드가 0에 수렴하면 프로그레스를 0으로 만듦
            hpBar.OnShieldHit(Mathf.Epsilon >= currShield ? 0f : shieldProgress, latePlay);

            Debug.Log("가중치 1미만");
        }

        // 맞기전 실드 + 체력 가중치가 1보다 오버면
        else if (1f <= prevTotalProgress && 0f < prevShield)
        {
            float total = (currShield + currHp);
            float hpRatio = currHp / total;

            hpBar.DirectShieldSet(1f);
            hpBar.CalcMain(hpRatio);

            Debug.Log("가중치 1이상");
        }
        else
        {
            hpBar.DirectShieldSet(0f);
            hpBar.OnHit(hpProgress);
        }

        if (null != hpText)
            hpText.OnHit(prevHp, currHp, hpProgress, damage, _damagNum: damagePool.Pool.Get());
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
        int currentEnemyCnt = waveSystemData.GetCurrentWaveProgress();
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
        if (!WaveStartFirstTime)
            return;

        hpText?.Init(playerData.GetCurrentHealth(), this);
        hpBar?.Init(playerData.GetCurrentHealth() / playerData.GetMaxHealth());
        targetBar?.Init(0f, waveSystemData.GetMaxWaveProgress());
        targetGageText?.DataUpdate(0f, waveSystemData.GetMaxWaveProgress());

        WaveStartFirstTime = false;
    }

    public void ReturnDamageText(GameObject target) => damagePool?.Pool.Release(target);
    public GameObject GetDamageObj() => damagePool.Pool.Get();
}
