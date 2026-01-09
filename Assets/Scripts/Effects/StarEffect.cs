using DG.Tweening;
using System;
using System.Diagnostics.Tracing;
using UnityEngine;

public class StarEffect : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform visual = null;
    [SerializeField] private float rotateDuration = 1f;

    private PoolingSystem poolingSystem = null;

    private TrailRenderer trail = null;

    private Sequence activeSeq = null;
    private Tween activeRotate = null;

    private CardDataInstance cardDataInstance = null;
    public CardDataInstance CardDataInstance { set { cardDataInstance = value; } }

    private Transform originParent = null; 

    public void Init(PoolingSystem _poolingSystem)
    {
        originParent = transform.parent;
        poolingSystem = _poolingSystem;
        trail = gameObject.GetComponentInChildren<TrailRenderer>();
    }

    public void AttachTo(Transform _target)
    {
        transform.position = _target.position;
        transform.SetParent(_target);
    }

    public void ReturnToOrigin() => transform.SetParent(originParent);

    public void PlayingEventforDeck(int _current, int _last, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        Action deckStartedEvent = () =>
        {
            UIView_CardSystem cardSystem = poolingSystem?.CardSystem;
            cardSystem?.PlayDrawedEffect();
        };

        Action deckCompoleteEvent = () =>
        {
            UIView_CardSystem cardSystem = poolingSystem?.CardSystem;
            cardSystem?.CallOneCardDrawed(_current, _last, transform.position, cardDataInstance, gameObject);
        };

        ExecuteMotionSeuence(_current, _spawnDelay, _drawDuration, _drawEase, points, deckStartedEvent, deckCompoleteEvent);
    }

    public void PlayingEventforWormHole(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        ExecuteMotionSeuence(_idx, _spawnDelay, _drawDuration, _drawEase, points);
    }

    private void ExecuteMotionSeuence(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points,
    Action _onExtraStart = null, Action _onExtraComplete = null)
    {
        if (null != activeSeq && activeSeq.IsActive())
            activeSeq.Kill();

        trail?.Clear();

        activeSeq = DOTween.Sequence();
        activeSeq.AppendInterval(_idx * _spawnDelay);
        activeSeq.Append(transform.DOPath(points, _drawDuration, PathType.CubicBezier, PathMode.TopDown2D, 10, Color.green)
            .SetUpdate(false)
            .SetEase(_drawEase)
            .OnStart(() =>
            {
                gameObject.SetActive(true);
                LoopRotate();

                _onExtraStart?.Invoke();
            })
            .OnComplete(() =>
            {
                activeRotate.Kill();

                _onExtraComplete?.Invoke();
            }));
    }

    private void LoopRotate()
    {
        if (null != activeRotate && activeRotate.IsActive())
            activeRotate.Kill();

        activeRotate = visual.DORotate(new Vector3(0f, 0f, 360f), rotateDuration, RotateMode.FastBeyond360)
            .SetUpdate(false)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}
 