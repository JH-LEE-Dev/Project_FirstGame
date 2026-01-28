using DG.Tweening;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

using Sequence = DG.Tweening.Sequence;

[Serializable]
public class DamageNumUnitValue
{
    [Header("Basic On Motion Settings")]
    public bool bOnShake = false;
    public bool bAlphaStart = false;
    public float onDuration = 0.4f;
    public float onShakePower = 5f;
    public float onAlpha = 0f;
    public Vector2 onStartSize = Vector2.one;
    public Vector2 onFinishedSize = Vector2.one;
    public Vector3 onPosOffset = Vector3.zero;
    public Ease onEase = Ease.Linear;

    [Header("Basic Off Motion Settings")]
    public float offDelay = 0.5f;
    public float offDuration = 0.4f;
    public Ease offEase = Ease.Linear;
}

public class DamageNumUnit : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Transform visual;
    private float data = 0f;
    public float DataValue { get { return data; } }

    private DamageNumberSystem owner;
    private DamageNumUnitValue unitValue;

    private Sequence visualSeq;

    private Vector3 originRot = Vector3.zero;
    private Vector3 originScale = Vector3.one;
    private Vector3 originPos = Vector3.zero;

    private bool bSumMotion = false;
    private Vector3 targetPos = Vector3.zero;

    public void SetupData(float _value) => data = _value;

    public void SetupUnitValue(DamageNumUnitValue _value, DamageNumberSystem dnbs)
    {
        owner = dnbs;

        unitValue = _value;
        Initialize();
    }

    public void BasicSpawnUnit(float _value, Vector3 _spawnWorldPos)
    {
        SetupData(_value);
        visual.position = _spawnWorldPos;
        data = _value;

        text.text = data.ToString();

        OnDamageNumber();
    }

    public void SumMotionSpawnUnit(float _value, Vector3 _spawnWorldPos, Vector3 _targetWorldPos)
    {
        BasicSpawnUnit(_value, _spawnWorldPos);

        bSumMotion = true;
        targetPos = _targetWorldPos;
        data = _value;

        text.text = data.ToString();

        OnDamageNumber();
    }

    private void OnDamageNumber()
    {
        CancelPrevMotion(visualSeq);

        visual.localScale = unitValue.onStartSize;

        if (unitValue.bAlphaStart)
        {
            text.alpha = unitValue.onAlpha;
        }
        else
        {
            text.alpha = 1f;
        }

        visualSeq = DOTween.Sequence();

        visualSeq.Append(visual.DOScale(unitValue.onFinishedSize, unitValue.onDuration)
            .SetEase(unitValue.onEase));

        if (unitValue.bAlphaStart)
        {
            visualSeq.Join(text.DOFade(1f, unitValue.onDuration)
                .SetEase(Ease.OutExpo));
        }

        if (unitValue.bOnShake)
        {
            visualSeq.Append(visual.DOShakePosition(unitValue.offDelay, unitValue.onShakePower)
                .SetEase(unitValue.onEase));
        }

        visualSeq.OnComplete(OnDamageNumComplete);
        visualSeq.SetUpdate(false);
    }

    private void OnDamageNumComplete()
    {
        if (bSumMotion)
            FinishedMotion_Sum();

        else
            FinishedMotion_Basic();
    }

    private void FinishedMotion_Sum()
    {
        CancelPrevMotion(visualSeq);

        visualSeq = DOTween.Sequence();

        // 구현 예정.
    }

    private void FinishedMotion_Basic()
    {
        CancelPrevMotion(visualSeq);

        visualSeq = DOTween.Sequence();

        visualSeq.Append(text.DOFade(0f, unitValue.offDuration)
            .SetEase(unitValue.offEase));

        visualSeq.OnComplete(FinishedMotionComplete);
    }

    private void FinishedMotionComplete()
    {
        owner?.ReleaseUnit(gameObject);
    }

    private void CancelPrevMotion(Sequence seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        seq = null;
    }

    private void Initialize()
    {
        if (null == visual)
            return;

        originRot = visual.localEulerAngles;
        originScale = visual.localScale;
        originPos = visual.localPosition;
    }

    private void OnDisable()
    {
        visualSeq.Kill();
    }
}
