using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AppUI.UI;
using UnityEditor;
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
    [SerializeField] private ConditionUI playerConditionUI;


    private bool WaveStartFirstTime = true;

    protected override void Awake()
    {
        base.Awake();

        if (uiPrefab != null)
            Instantiate(uiPrefab, uiRoot);
    }

    private void OnDisable()
    {
        if (null != playerData)
            playerData.PlayerDebuffChangedEvent -= Init_ConditionUI;
    }

    public void DataInjection(IWaveSystemData _waveSystemData, IPlayerData _playerData,ICharacterData _characterData)
    {
        waveSystemData = _waveSystemData;
        playerData = _playerData;
        characterData = _characterData;

        IntializeChildrenHUD();
        Init_CharacterStat();
        Init_ConditionUI();

        if (null != playerData)
        {
            playerData.PlayerDebuffChangedEvent -= Init_ConditionUI;
            playerData.PlayerDebuffChangedEvent += Init_ConditionUI;
        }
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
        WaveAdjustment();
        waveStateDeclareText.gameObject.SetActive(true);
        waveStateDeclareText.text = "Prepare For Next Wave!!";
    }

    public void PlayerTurnStarted()
    {
        TurnAdjustment();

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

    public void PlayerEarnMoney(int amount)
    {
        ActivateSubUI(StarLightAcquisitionType.Kill, amount);
    }

    public void WaveRewardRecieved(int amount)
    {
        ActivateSubUI(StarLightAcquisitionType.OverKill, amount);
    }

    public void PlayerGetShield(float amount)
    {
        HP_BarShieldCalc();
        ShieldTextUpdate(amount);
    }

    public void PlayerGetHP(float amount)
    {
        HP_BarShieldCalc();
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

    public void OnPlayerHit(float damage)
    {
        HP_BarUpdateforDamaged(damage);
    }

    private void HP_BarUpdateLateHit()
    {
        float maxHp = playerData.GetMaxHealth();
        float currHp = playerData.GetCurrentHealth();

        float hpProgress = Mathf.Clamp(currHp / maxHp, 0f, 1f);

        hpBar.OnHit(hpProgress);
    }

    private void HP_BarUpdateforDamaged(float damage)
    {
        if (null == hpBar || null == playerDamageNumPool)
            return;

        float maxHp = playerData.GetMaxHealth();
        float prevHp = playerData.GetPrevHealth();
        float currHp = playerData.GetCurrentHealth();
        float currShield = playerData.GetCurrentShield();
        float prevShield = playerData.GetPrevShield();

        float prevTotalProgress = (prevShield + prevHp) / maxHp;
        float shieldProgress = Mathf.Clamp((currShield + currHp) / maxHp, 0f, 1f);
        float hpProgress = Mathf.Clamp(currHp / maxHp, 0f, 1f);

        // 맞기 전에 쉴드가 있었다면
        if (0f < prevShield)
        {
            // 맞은 다음 쉴드가 아예 없고, 맞은 다음의 쉴드 가중치가 최대 체력 이하일 때
            if (0f >= currShield && 1f >= shieldProgress)
                hpBar.OnShieldHit(0f, HP_BarUpdateLateHit);

            // 현재 쉴드가 남았을 때, 현재 쉴드 가중치가 최대 체력과 동일하거나 넘으면
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

        // 맞기 전에 쉴드가 없었으면 평범하게 맞기
        else
        {
            hpBar.DirectShieldSet(0f);
            hpBar.OnHit(hpProgress);
        }

        if (null != hpText)
            hpText.OnHit(prevHp, currHp, hpProgress, damage, prevShield, currShield, _damagNum: playerDamageNumPool.Pool.Get());
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
            hpBar.CalcMain(Mathf.Clamp(currHp / maxHp, 0f, 1f), true);
            hpBar.CalcShield(Mathf.Clamp(shieldProgress, 0f, 1f));
        }
        else
        {
            float hpRatio = Mathf.Clamp(currHp / maxHp, 0f, 1f);

            if (1f <= hpRatio)
                hpBar.CalcMain(hpRatio);
            else
                hpBar.CalcMain(hpRatio, callback:ShieldZero);
        }

        hpText.CalcHP(currHp);
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
        if (null == target)
            return;

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
        characterStatUI.ChangeValue(PlayerStatType.AttackDamage, stat.resultDamage);
        characterStatUI.ChangeValue(PlayerStatType.AdditionalDamage, 0f);
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
        characterStatUI.Setup(PlayerStatType.AttackDamage, "기본 공격력:", stat.resultDamage);
        characterStatUI.Setup(PlayerStatType.AdditionalDamage, "추가 공격력:", 0f);
        characterStatUI.Setup(PlayerStatType.WeaknessTurnCount, "적 약화 디버프 횟수:", stat.weaknessTurnCnt);
    }

    private void Init_ConditionUI()
    {
        if (null == playerConditionUI || null == playerData)
            return;

        playerConditionUI.UpdateConditions(playerData.currentAppliedDebuff);
    }

    // For StarlightUI

    [NaughtyAttributes.Button]
    void Test1()
    {
        ActivateSubUI(StarLightAcquisitionType.Kill, 10);
    }
    [NaughtyAttributes.Button]
    void Test2()
    {
        ActivateSubUI(StarLightAcquisitionType.Ability, 10);
    }
    [NaughtyAttributes.Button]
    void Test3()
    {
        ActivateSubUI(StarLightAcquisitionType.OverKill, 10);
    }

    [NaughtyAttributes.Button]
    void Test4()
    {
        TurnAdjustment();
    }

    [NaughtyAttributes.Button]
    void Test5()
    {
        WaveAdjustment();
    }



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
