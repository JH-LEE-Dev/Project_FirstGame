using System;
using DamageNumbersPro;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

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
    [ShowIf("colorChange"), SerializeField] private Color startColor = Color.softRed;
    [ShowIf("colorChange"), SerializeField] private Color finalColor = Color.white;
    [Space]
    [ShowIf("colorChange"), SerializeField] private Color shieldStartColor = Color.softRed;
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

    private Vector2 originalAnchoredPos = Vector2.zero;

    private Vector2 originShieldAbchoredPos = Vector2.zero;
    private Vector2 spawnShieldAbchoredPos = Vector2.zero;

    private void Awake()
    {
        if(null != visualRect)
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

        if (hpText == null || (typeof(T) != typeof(int) && typeof(T) != typeof(float)))
            return;

        float convertedValue = Convert.ToSingle(_value);
        hpText.text = Mathf.RoundToInt(convertedValue).ToString();

        shieldText?.gameObject.SetActive(false);
    }

    public void CalcShield(float _prev, float _current, Action completed = null)
    {
        if (null == shieldText)
            return;

        if (0f >= _prev)
        {
            shieldText.gameObject.SetActive(true);
            shieldText.rectTransform.anchoredPosition = spawnShieldAbchoredPos;
            shieldText.alpha = 0f;

            shieldText.rectTransform.DOAnchorPos(originShieldAbchoredPos, spawnShieldDuration)
                .SetEase(spawnShieldEase);

            shieldText.DOFade(1f, spawnShieldDuration)
                .SetEase(spawnShieldEase);
        }

        DOVirtual.Float(_prev, _current, motionDuration, (value) =>
        {
            if (0f < value)
                shieldText.text = "+";
            else
            {
                shieldText.text = "";
                shieldText.gameObject.SetActive(false);
            }

            shieldText.text += Mathf.RoundToInt(value).ToString();
        })
            .SetEase(motionEase)
            .SetUpdate(false)
            .OnComplete(() =>
            {
                completed?.Invoke();
            });
    }

    private void CalcHP(float _prev, float _current)
    {
        DOVirtual.Float(_prev, _current, motionDuration, (value) =>
        {
            hpText.text = Mathf.RoundToInt(value).ToString();
        }).SetEase(motionEase).SetUpdate(false);
    }

    public void OnHit(float _prevHp, float _currHp, float _hpProgress, float _damage, 
        float _prevShield, float _currShield, GameObject _damagNum = null)
    {
        if (null == hpText || null == shieldText)
            return;

        if (!damageNumber)
            OnDefaultHit(_prevHp, _currHp, _hpProgress, _damage, _prevShield, _currShield);
        else
            OnDamageNumberHit(_prevHp, _currHp, _damage, _hpProgress, _prevShield, _currShield, _damagNum);
    }

    private void OnDefaultHit(float _prevHp, float _currHp, float _progressHp, float _damage, 
        float _prevShield, float _currShield)
    {
        bool shield = 0f < _prevShield;

        if (shield)
        {
            Action remainDamage = () =>
            {
                if (0f < _damage - _prevShield)
                {
                    CalcHP(_prevHp, _currHp);
                    OnColorChange(_progressHp, false);
                }
            };

            CalcShield(_prevShield, _currShield, remainDamage);
        }
        else
            CalcHP(_prevHp, _currHp);

        OnColorChange(_progressHp, shield);
        OnShake();
    }

    private void OnDamageNumberHit(float _prev, float _current, float _damage, float _progress, 
        float _prevShield, float _currShield, GameObject _damagNum)
    {
        UIText_DamageNumPlayer script = _damagNum?.GetComponent<UIText_DamageNumPlayer>();
        if (null == script || null == visualRect)
            return;

        string damageString = "-" + Mathf.RoundToInt(_damage).ToString();
        script.Setup(damageString, damageWait, damageSpawnPoint.position, damageEndPoint);

        Action callback = () =>
        {
            OnDefaultHit(_prev, _current, _progress, _damage, _prevShield, _currShield);
            hudSystem?.ReturnDamageText(_damagNum);
        };

        bool dangerDamage = 0.5f <= (_damage / _prev);

        script.PlayMotion(dangerDamage, callback);
    }

    private void OnColorChange(float _progress, bool _shield)
    {
        if (!colorChange)
            return;

        colorSeq = CancelPrevMotion(colorSeq);

        if (_shield)
            ShieldTextColorChanging();

        else
            HPTextColorChanging(_progress);
    }

    private void ShieldTextColorChanging()
    {
        colorSeq.AppendCallback(() =>
        {
            shieldText.color = shieldStartColor;
        });

        colorSeq.Append(shieldText.DOColor(shieldFinalColor, motionDuration)
            .SetEase(motionEase)
            .SetUpdate(false));
    }

    private void HPTextColorChanging(float _progress)
    {
        Color targetColor = finalColor;

        if (overrideColor)
        {
            if (0.25f >= _progress)
                targetColor = dangerColor;
            else if (0.5f >= _progress)
                targetColor = warningColor;
        }

        colorSeq.AppendCallback(() =>
        {
            hpText.color = startColor;
        });

        colorSeq.Append(hpText.DOColor(targetColor, motionDuration)
            .SetEase(motionEase)
            .SetUpdate(false));
    }

    private void OnShake()
    {
        if (!shaking || null == visualRect)
            return;

        shakeSeq = CancelPrevMotion(shakeSeq);

        shakeSeq.AppendCallback(() =>
        {
            visualRect.anchoredPosition = originalAnchoredPos;
        });

        shakeSeq.Append(visualRect.DOShakeAnchorPos(shakeDuration, shakePower)
            .SetEase(shakeEase)
            .SetUpdate(false));
    }

    private Sequence CancelPrevMotion(Sequence target)
    {
        if (target.IsActive())
            target.Kill();

        return DOTween.Sequence();
    }
}
