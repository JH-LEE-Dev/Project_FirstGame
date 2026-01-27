using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using System;

public class UIText_PlayerHP : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI shieldText;
    [SerializeField] private float motionDuration = 1f;
    [SerializeField] private Ease motionEase = Ease.Linear;

    [SerializeField] private bool colorChange = false;
    [SerializeField] private bool shaking = false;
    [SerializeField] private bool damageNumber = false;
    private UIView_HUD hudSystem = null;

    [Header("Shield Settings")]
    [SerializeField] private float spawnShieldDuration = 0.25f;
    [SerializeField] private float spawnShieldX = 30f;
    [SerializeField] private Ease spawnShieldEase = Ease.OutExpo;

    [Header("Color Change Settings")]
    [ShowIf("colorChange"), SerializeField] private bool overrideColor = false;
    [ShowIf("colorChange"), SerializeField] private Color startColor = Color.red;
    [ShowIf("colorChange"), SerializeField] private Color finalColor = Color.white;
    [Space]
    [ShowIf("colorChange"), SerializeField] private Color shieldStartColor = Color.red;
    [ShowIf("colorChange"), SerializeField] private Color shieldFinalColor = Color.white;

    [Header("Override FinalColor Settings")]
    [ShowIf("overrideColor"), SerializeField] private Color warningColor = Color.yellow;
    [ShowIf("overrideColor"), SerializeField] private Color dangerColor = Color.orangeRed;

    [Header("Shaking Settings")]
    [ShowIf("shaking"), SerializeField] private RectTransform visualRect = null;
    [ShowIf("shaking"), SerializeField] private float shakeDuration = 1f;
    [ShowIf("shaking"), SerializeField] private float shakePower = 5f;
    [ShowIf("shaking"), SerializeField] private Ease shakeEase = Ease.Linear;

    [Header("Damage Number")]
    [ShowIf("damageNumber"), SerializeField] private float damageWait = 0.5f;
    [ShowIf("damageNumber"), SerializeField] private RectTransform damageSpawnPoint;
    [ShowIf("damageNumber"), SerializeField] private RectTransform damageEndPoint;

    private Sequence colorSeq = null;
    private Sequence shakeSeq = null;
    private Tween shieldTween = null;
    private Tween hpTween = null;

    private Vector2 originalAnchoredPos = Vector2.zero;
    private Vector2 originShieldAbchoredPos = Vector2.zero;
    private Vector2 spawnShieldAbchoredPos = Vector2.zero;

    // [최적화] 람다 캡처 방지를 위한 임시 변수들
    private Action onShieldCompletedEvent;
    private float tempPrevHp;
    private float tempCurrHp;
    private float tempProgressHp;
    private float tempDamage;
    private float tempPrevShield;
    private float tempCurrShield;
    private GameObject tempDamageNum;

    private void Awake()
    {
        if (null != visualRect)
            originalAnchoredPos = visualRect.anchoredPosition;

        if (null != shieldText)
        {
            originShieldAbchoredPos = shieldText.rectTransform.anchoredPosition;
            spawnShieldAbchoredPos = originShieldAbchoredPos;
            spawnShieldAbchoredPos.x += spawnShieldX;
        }
    }

    public void Init<T>(T _value, UIView_HUD _hudSystem) where T : struct
    {
        hudSystem = _hudSystem;

        if (hpText == null) 
            return;

        float convertedValue = Convert.ToSingle(_value);
        hpText.text = Mathf.RoundToInt(convertedValue).ToString();

        shieldText?.gameObject.SetActive(false);
    }

    public void CalcShield(float _prev, float _current, Action completed = null)
    {
        if (null == shieldText) 
            return;

        onShieldCompletedEvent = completed;

        if (0f >= _prev && _current > 0f)
        {
            shieldText.gameObject.SetActive(true);
            shieldText.rectTransform.anchoredPosition = spawnShieldAbchoredPos;
            shieldText.alpha = 0f;

            shieldText.rectTransform.DOAnchorPos(originShieldAbchoredPos, spawnShieldDuration).SetEase(spawnShieldEase);
            shieldText.DOFade(1f, spawnShieldDuration).SetEase(spawnShieldEase);
        }

        if (shieldTween != null && shieldTween.IsActive()) shieldTween.Kill();

        // 람다 대신 메서드(UpdateShieldText) 호출
        shieldTween = DOVirtual.Float(_prev, _current, motionDuration, UpdateShieldText)
            .SetEase(motionEase)
            .SetUpdate(false)
            .OnComplete(OnShieldTweenComplete);
    }

    private void UpdateShieldText(float value)
    {
        if (value > 0f)
            shieldText.text = "+" + Mathf.RoundToInt(value).ToString();
        else
        {
            shieldText.text = "";
            if (shieldText.gameObject.activeSelf)
                shieldText.gameObject.SetActive(false);
        }
    }

    private void OnShieldTweenComplete()
    {
        onShieldCompletedEvent?.Invoke();
        onShieldCompletedEvent = null;
    }

    private void CalcHP(float _prev, float _current)
    {
        if (hpTween != null && hpTween.IsActive()) hpTween.Kill();

        hpTween = DOVirtual.Float(_prev, _current, motionDuration, UpdateHPText)
            .SetEase(motionEase)
            .SetUpdate(false);
    }

    private void UpdateHPText(float value)
    {
        hpText.text = Mathf.RoundToInt(value).ToString();
    }

    public void OnHit(float _prevHp, float _currHp, float _hpProgress, float _damage,
        float _prevShield, float _currShield, GameObject _damagNum = null)
    {
        if (null == hpText || null == shieldText) 
            return;

        // 멤버 변수에 저장 (콜백에서 쓰기 위해)
        tempPrevHp = _prevHp;
        tempCurrHp = _currHp;
        tempProgressHp = _hpProgress;
        tempDamage = _damage;
        tempPrevShield = _prevShield;
        tempCurrShield = _currShield;
        tempDamageNum = _damagNum;

        if (!damageNumber)
        {
            OnDefaultHit();
        }
        else
        {
            OnDamageNumberHit();
        }
    }

    private void OnDefaultHit()
    {
        bool hasShield = tempPrevShield > 0f;

        if (hasShield)
            CalcShield(tempPrevShield, tempCurrShield, OnShieldCalcFinished);
        else
            CalcHP(tempPrevHp, tempCurrHp);

        OnColorChange(tempProgressHp, hasShield);
        OnShake();
    }

    private void OnShieldCalcFinished()
    {
        if (0f < tempDamage - tempPrevShield)
        {
            CalcHP(tempPrevHp, tempCurrHp);
            OnColorChange(tempProgressHp, false);
        }
    }

    private void OnDamageNumberHit()
    {
        UIText_DamageNumPlayer script = tempDamageNum?.GetComponent<UIText_DamageNumPlayer>();
        if (null == script || null == visualRect) return;

        string damageString = "-" + Mathf.RoundToInt(tempDamage).ToString();
        script.Setup(damageString, damageWait, damageSpawnPoint.position, damageEndPoint);

        bool dangerDamage = (tempDamage / tempPrevHp) >= 0.5f;

        // 콜백으로 메서드 전달
        script.PlayMotion(dangerDamage, OnDamageNumberComplete);
    }

    private void OnDamageNumberComplete()
    {
        OnDefaultHit();
        hudSystem?.ReturnDamageText(tempDamageNum);
        tempDamageNum = null;
    }

    private void OnColorChange(float _progress, bool _isShield)
    {
        if (!colorChange) 
            return;

        if (colorSeq != null && colorSeq.IsActive()) colorSeq.Kill();
        colorSeq = DOTween.Sequence();

        if (_isShield)
        {
            shieldText.color = shieldStartColor;
            colorSeq.Append(shieldText.DOColor(shieldFinalColor, motionDuration).SetEase(motionEase));
        }
        else
        {
            Color targetColor = finalColor;
            if (overrideColor)
            {
                if (_progress <= 0.25f) targetColor = dangerColor;
                else if (_progress <= 0.5f) targetColor = warningColor;
            }

            hpText.color = startColor;
            colorSeq.Append(hpText.DOColor(targetColor, motionDuration).SetEase(motionEase));
        }

        colorSeq.SetUpdate(false);
    }

    private void OnShake()
    {
        if (!shaking || null == visualRect) 
            return;

        if (shakeSeq != null && shakeSeq.IsActive()) shakeSeq.Kill();
        shakeSeq = DOTween.Sequence();

        visualRect.anchoredPosition = originalAnchoredPos;

        shakeSeq.Append(visualRect.DOShakeAnchorPos(shakeDuration, shakePower).SetEase(shakeEase));
        shakeSeq.SetUpdate(false);
    }
}