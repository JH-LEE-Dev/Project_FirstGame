using DG.Tweening;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;

public class UIText_DamageNumPlayer : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TMP_Text mainText;
    [SerializeField] private RectTransform visualRect;
    [SerializeField] private bool collision;
    [SerializeField] private Color defaultColor = Color.orange;
    [SerializeField] private Color dangerColor = Color.red;

    [Header("First Settings")]
    [SerializeField] private Vector2 Dir;
    [SerializeField] private float power = 1f;
    [SerializeField] private float firstWait = 1f;

    [Header("Final Settings")]
    [SerializeField] private float finalDuration = 1f;
    [SerializeField] private Ease finalEase = Ease.Linear;

    private Rigidbody2D rigid;
    private Collider2D coll;

    private float waitSecond = 0f;
    private Vector3 startPosition;
    private RectTransform target;

    private Vector3 originScale;

    private Sequence seq;

    private Action playMotionCompleteEvent;

    private void Awake()
    {
        originScale = visualRect.localScale;

        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void Start()
    {
        if (null == visualRect)
            visualRect = mainText?.gameObject.GetComponent<RectTransform>();

        if (null != coll)
            coll.isTrigger = !collision;
    }

    public void Setup(string _text, float _waitSecond, Vector3 _startPosition, RectTransform _target)
    {
        startPosition = _startPosition;
        target = _target;
        waitSecond = _waitSecond;

        if (null != mainText)
        {
            mainText.text = _text;
            mainText.alpha = 1f;
        }

        gameObject.SetActive(true);
    }

    public void PlayMotion(bool _danger, Action _callback = null)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        playMotionCompleteEvent = _callback;

        seq = DOTween.Sequence();

        seq.AppendInterval(waitSecond);

        FirstMotion(_danger);
        FinaltMotion();

        seq.SetUpdate(false);
        seq.OnComplete(PlayMotionCompleteEvent);
    }

    private void FirstMotion(bool _danger)
    {
        if (null != rigid)
            rigid.simulated = true;

        if (null != visualRect)
        {
            visualRect.position = startPosition;
            visualRect.localScale = originScale;
            mainText.color = defaultColor;

            if (_danger)
            {
                mainText.color = dangerColor;
                visualRect.localScale *= 1.25f;
            }
        }

        rigid.AddForce(Dir.normalized * (Random.Range(0.5f, 1f) * power), ForceMode2D.Impulse);
        rigid.AddTorque(Random.Range(0.01f, 0.05f), ForceMode2D.Impulse);
        seq.AppendInterval(firstWait);
    }

    private void FinaltMotion()
    {
        Vector2 targetPos = UIWorldUtil.GetAnchoredPosToTarget(target, visualRect);

        Debug.Log(targetPos);

        seq.Append(visualRect.DOAnchorPos(targetPos, finalDuration)
            .SetEase(finalEase)
            .OnStart(FinalMotionStartEvent));

        seq.Join(visualRect.DOScale(0f, finalDuration)
            .SetEase(finalEase));

        seq.Join(mainText.DOFade(0f, finalDuration)
            .SetEase(finalEase));
    }

    private void PlayMotionCompleteEvent()
    {
        playMotionCompleteEvent?.Invoke();
        playMotionCompleteEvent = null;
    }

    private void FinalMotionStartEvent()
    {
        if (null != rigid)
            rigid.simulated = false;
    }
}
