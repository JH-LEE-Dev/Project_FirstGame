using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

using Sequence = DG.Tweening.Sequence;

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
    [SerializeField] private RectTransform visualRect;
    private float data = 0f;
    public float DataValue { get { return data; } }

    private DamageNumUnitValue unitValue;

    private Sequence visualSeq;

    private Vector3 originRot = Vector3.zero;
    private Vector3 originScale = Vector3.one;
    private Vector3 originPos = Vector3.zero;

    private bool bSumMotion = false;
    private Vector3 targetPos = Vector3.zero;

    public void SetupData(float _value) => data = _value;

    public void SetupUnitValue(DamageNumUnitValue _value)
    {
        unitValue = _value;
        Initialize();
    }

    public void BasicSpawnUnit(float _value, Vector3 _spawnWorldPos)
    {
        SetupData(_value);
        visualRect.anchoredPosition = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(_spawnWorldPos, visualRect);

        OnDamageNumber();
    }

    public void SumMotionSpawnUnit(float _value, Vector3 _spawnWorldPos, Vector3 _targetWorldPos)
    {
        BasicSpawnUnit(_value, _spawnWorldPos);

        bSumMotion = true;
        targetPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(_targetWorldPos, visualRect);

        OnDamageNumber();
    }

    private void OnDamageNumber()
    {
        CancelPrevMotion(visualSeq);

        visualSeq = DOTween.Sequence();

        visualSeq.AppendCallback(OnDamageNumCallback);

        visualSeq.Append(visualRect.DOScale(unitValue.onFinishedSize, unitValue.onDuration)
            .SetEase(unitValue.onEase));

        if (unitValue.bAlphaStart)
        {
            visualSeq.Join(text.DOFade(1f, unitValue.onDuration)
                .SetEase(Ease.OutExpo));
        }
        else
        {
            text.alpha = 1f;
        }

        if (unitValue.bOnShake)
        {
            visualSeq.Append(visualRect.DOShakeAnchorPos(unitValue.offDelay, unitValue.onShakePower)
                .SetEase(unitValue.onEase));
        }

        visualSeq.OnComplete(OnDamageNumComplete);
        visualSeq.SetUpdate(false);
    }

    private void OnDamageNumCallback()
    {
        visualRect.localScale = unitValue.onStartSize;

        if (unitValue.bAlphaStart)
        {
            text.alpha = unitValue.onAlpha;
        }
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
    }

    private void CancelPrevMotion(Sequence seq)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        seq = null;
    }

    private void Initialize()
    {
        if (null == visualRect)
            return;

        originRot = visualRect.localEulerAngles;
        originScale = visualRect.localScale;
        originPos = visualRect.localPosition;
    }

    [Button]
    private void BasicTestButton()
    {
        unitValue.bOnShake = true;
        unitValue.onStartSize = new Vector2(2, 2);

        BasicSpawnUnit(10, Vector3.zero);
    }
}
