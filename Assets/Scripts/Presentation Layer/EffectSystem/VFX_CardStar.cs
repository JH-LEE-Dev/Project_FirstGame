using DG.Tweening;
using System;
using System.Diagnostics.Tracing;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class VFX_CardStar : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform visual = null;
    [SerializeField] private float rotateDuration = 1f;

    private PoolingSystem poolingSystem = null;

    private ParticleSystem[] particles;

    private Sequence activeSeq = null;
    private Tween activeRotate = null;

    private CardDataInstance cardDataInstance = null;
    public CardDataInstance CardDataInstance { set { cardDataInstance = value; } }

    public void Init(PoolingSystem _poolingSystem)
    {
        poolingSystem = _poolingSystem;

        particles = GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem vfx in particles)
        {
            var main = vfx.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Custom;
            main.customSimulationSpace = GetComponentInParent<Canvas>().transform;
        }
    }

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

    public void PlayingEventforGraveToHands(int _current, int _last, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        Action deckCompoleteEvent = () =>
        {
            UIView_CardSystem cardSystem = poolingSystem?.CardSystem;
            cardSystem?.CallOneCardDrawed(_current, _last, transform.position, cardDataInstance, gameObject);
        };

        ExecuteMotionSeuence(_current, _spawnDelay, _drawDuration, _drawEase, points, null, deckCompoleteEvent);
    }

    public void PlayingEventforWormHole(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        Action deckStartedEvent = () =>
        {
            UIView_CardSystem cardSystem = poolingSystem?.CardSystem;
            cardSystem?.PlayMoveToDeckMotion();
        };

        Action deckCompoleteEvent = () =>
        {
            UIView_CardSystem cardSystem = poolingSystem?.CardSystem;
            cardSystem?.CallGraveToDeckFinished(_idx, gameObject);
        };

       ExecuteMotionSeuence(_idx, _spawnDelay, _drawDuration, _drawEase, points, deckStartedEvent, deckCompoleteEvent);
    }

    private void ExecuteMotionSeuence(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points,
    Action _onExtraStart = null, Action _onExtraComplete = null)
    {
        if (null != activeSeq && activeSeq.IsActive())
            activeSeq.Kill();

        foreach (ParticleSystem vfx in particles)
            vfx?.Play(true);

        activeSeq = DOTween.Sequence();
        activeSeq.AppendInterval(_idx * _spawnDelay);
        activeSeq.Append(transform.DOLocalPath(points, _drawDuration, PathType.CubicBezier, PathMode.TopDown2D, 70, Color.green)
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
 