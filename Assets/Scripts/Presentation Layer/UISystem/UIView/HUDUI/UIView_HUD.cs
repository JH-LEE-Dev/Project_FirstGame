using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class UIView_HUD : UIView
{
    //외부 데이터
    IPlayerData playerData;
    ICharacterData characterData;
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

    [Header("DamageNumber")]
    [SerializeField] private DamageNumberSystem damageNumberSystem;

    [Header("Pooling System")]
    [SerializeField] private ObjectPoolingSystem playerDamageNumPool;
    [SerializeField] private ObjectPoolingSystem targetBarEffectPool;

    [Header("StarlightUI")]
    [SerializeField] private StarlightUI starlight;

    [Header("CharacterStatUI")]
    [SerializeField] private UIStat_Player characterStatUI;


    private bool WaveStartFirstTime = true;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);
    }

    private void Start()
    {
        
    }

    public void DataInjection(IWaveSystemData _waveSystemData, IPlayerData _playerData,ICharacterData _characterData)
    {
        waveSystemData = _waveSystemData;
        playerData = _playerData;
        characterData = _characterData;

        IntializeChildrenHUD();
        Init_CharacterStat();
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
        
    }

    public void EnemyIsKilled(Vector2 deadPosition)
    {
        Target_BarUpdate(deadPosition);
    }

    public void PlayerGetShield(float amount)
    {
        HP_BarShieldCalc();
        ShieldTextUpdate(amount);
    }

    public void PlayerGetHP(float amount)
    {

    }

    public void ResetPlayerShield()
    {
        HP_BarShieldCalc();
        ResetShieldText();
    }

    public void CharacterStatChanged()
    {
        CharacterStatUpdate();
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

        if (0f < prevShield)
        {
            if (0f >= currShield && 1f > shieldProgress)
            {
                Action latePlay = () =>
                {
                    hpBar.OnHit(hpProgress);
                };

                hpBar.OnShieldHit(0f, latePlay);
            }

            else if (0f < currShield && 1f <= shieldProgress)
            {
                float total = (currShield + currHp);
                float hpRatio = currHp / total;

                hpBar.DirectShieldSet(1f);
                hpBar.CalcMain(hpRatio);
            }

            else
                hpBar.OnShieldHit(shieldProgress);
        }

        else
        {
            hpBar.DirectShieldSet(0f);
            hpBar.OnHit(hpProgress);
        }

        if (null != hpText)
            hpText.OnHit(prevHp, currHp, hpProgress, damage, prevShield, currShield,_damagNum: playerDamageNumPool.Pool.Get());
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
        int currentKillCnt = maxEnemyCnt - currentEnemyCnt;

        float currentProgress = (float)currentKillCnt / maxEnemyCnt;

        script.SetupSavedData(currentProgress, currentKillCnt, maxEnemyCnt);
        script.Play(targetBar.GetAnchoredPos(), TargetBarCallbackEvent);
    }

    private void TargetBarCallbackEvent(VFX_TargetBarStar vfx)
    {
        if (null == vfx)
            return;

        targetBar.OnFill(vfx.savedCurrentProgress);

        targetGageText?.DataUpdate(vfx.savedCurrentKillCnt, vfx.savedEnemyMaxCnt);
        StartCoroutine(ReleaseEffect(vfx));
    }

    private void HP_BarShieldCalc()
    {
        if (null == hpBar)
            return;

        float maxHp = playerData.GetMaxHealth();
        float currHp = playerData.GetCurrentHealth();
        float currShield = playerData.GetCurrentShield();

        float total = (currShield + currHp);

        float shieldProgress = total / maxHp;

        if (0f < currShield && 1f < shieldProgress)
        {
            float hpRatio = currHp / total;

            hpBar.DirectShieldSet(1f);
            hpBar.CalcMain(hpRatio);
        }
        else if (0f < currShield)
        {
            hpBar.CalcShield(Mathf.Clamp(shieldProgress, 0f, 1f));
        }
        else
        {
            float hpRatio = Mathf.Clamp(currHp / maxHp, 0f, 1f);

            if (1f <= hpRatio)
                hpBar.CalcMain(hpRatio);
            else
                hpBar.CalcMain(hpRatio, ShieldZero);
        }
    }

    public void ShieldZero() => hpBar?.CalcShield(0f);

    private void ShieldTextUpdate(float _amount)
    {
        float currentShield = playerData.GetCurrentShield();
        float prevShield = currentShield - _amount;

        hpText?.CalcShield(prevShield, currentShield);
    }

    private void ResetShieldText()
    {
        float prevShield = playerData.GetPrevShield();
        hpText?.CalcShield(prevShield, 0f);
    }

    private IEnumerator ReleaseEffect(VFX_TargetBarStar target)
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (target.CheckAliveParticle())
            yield return wait;

        targetBarEffectPool.Pool.Release(target.gameObject);
    }

    private void IntializeChildrenHUD()
    {
        if (!WaveStartFirstTime)
            return;

        hpText?.Init(playerData.GetCurrentHealth(), this);
        hpBar?.Init(playerData.GetCurrentHealth() / playerData.GetMaxHealth());
        targetBar?.Init(0f, waveSystemData.GetMaxWaveProgress());
        targetGageText?.DataUpdate(0, waveSystemData.GetMaxWaveProgress());

        WaveStartFirstTime = false;
    }

    public void ReturnDamageText(GameObject target)
    {
        if(target.activeSelf)
        {
            playerDamageNumPool?.Pool.Release(target);
        }
    }

    public GameObject GetDamageObj() => playerDamageNumPool.Pool.Get();

    private void CharacterStatUpdate()
    {
        if (null == characterStatUI)
            return;

        ICharacterStatProvider stat = characterData.GetStatProvider();

        // 스탯 조정 됐을 때 ( 원래대로 돌아갈 때 / 변화를 가질 때 )
        characterStatUI.ChangeValue(PlayerStatType.AttackCount, stat.attackCnt);
        characterStatUI.ChangeValue(PlayerStatType.AttackRange, stat.attackRange);
        characterStatUI.ChangeValue(PlayerStatType.CriticalChance, stat.criticalChance);
        characterStatUI.ChangeValue(PlayerStatType.AttackDamage, stat.attack);
        characterStatUI.ChangeValue(PlayerStatType.WeaknessTurnCount, stat.weaknessTurnCnt);
    }

    private void Init_CharacterStat()
    {
        if (null == characterStatUI)
            return;

        ICharacterStatProvider stat = characterData.GetStatProvider();

        characterStatUI.Setup(PlayerStatType.AttackCount, "공격 횟수:", stat.attackCnt);
        characterStatUI.Setup(PlayerStatType.AttackRange, "공격 범위:", stat.attackRange);
        characterStatUI.Setup(PlayerStatType.CriticalChance, "치명타 확률:", stat.criticalChance);
        characterStatUI.Setup(PlayerStatType.AttackDamage, "공격력:", stat.attack);
        characterStatUI.Setup(PlayerStatType.WeaknessTurnCount, "적 약화 디버프 횟수:", stat.weaknessTurnCnt);
    }

    // For StarlightUI

    // StarLightAcquisitionType.Kill -> 적 유닛을 킬 하면 오르는 재화
    // StarLightAcquisitionType.Ability -> 카드 능력 혹은 서브위성으로 인한 추가 재화
    // StarLightAcquisitionType.OverKill -> Wave 클리어 충족치를 넘겼을 때, 그 만큼 버는 재화
    // addValue -> 더해질 재화
    public void ActivateSubUI(StarLightAcquisitionType type, int addValue)
    {
        starlight?.ActivateSubUI(type, addValue);
    }
    // 적 턴까지 전부 끝났을 때 한번 호출. (
    public void TurnAdjustment()
    {
        starlight?.TurnAdjustment();
    }
    // Wave 자체가 끝났을 때 호출. (최종 정산)
    public void WaveAdjustment()
    {
        starlight?.WaveAdjustment();
    }
    // UI가 가리키는 현재 자산
    public int GetStarlight()
    {
        if (!starlight) return -1;
        return starlight.GetStarlight();
    }
}
