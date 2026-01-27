using DG.Tweening;
using System;
using UnityEngine;

public class VFX_CardStar : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform visual = null;
    [SerializeField] private float rotateDuration = 1f;

    private PoolingSystem poolingSystem = null;
    private ParticleSystem[] particles;

    private Sequence activeSeq = null;
    private Tween activeRotate = null;

    private Vector3 targetPos = Vector3.zero;
    public Vector3 TargetPos => targetPos;

    private CardDataInstance cardDataInstance = null;
    public CardDataInstance CardDataInstance 
    { 
        get { return cardDataInstance; } 
        set { cardDataInstance = value; } 
    }

    private int tempCurrentIdx;
    private int tempLastIdx;

    private Action currentStartCallback;
    private Action currentCompleteCallback;

    private Action<VFX_CardStar> onStartCallbackWithParam;
    private Action<VFX_CardStar> onCompleteCallbackWithParam;

    public void Init(PoolingSystem _poolingSystem)
    {
        poolingSystem = _poolingSystem;
        particles = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem vfx in particles)
        {
            var main = vfx.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Custom;
            main.customSimulationSpace = GetComponentInParent<Canvas>().transform;
        }
    }

    public void PlayCardSpawnEvent(int _currIdx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points,
        Action<VFX_CardStar> _startEvent = null, Action<VFX_CardStar> _completeEvent = null)
    {
        tempCurrentIdx = _currIdx;

        onStartCallbackWithParam = _startEvent;
        onCompleteCallbackWithParam = _completeEvent;

        ExecuteMotionSequence(_currIdx, _spawnDelay, _drawDuration, _drawEase, points);
    }

    public void PlayingEventforDeck(int _current, int _last, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        tempCurrentIdx = _current;
        tempLastIdx = _last;

        currentStartCallback = OnDeckStart;
        currentCompleteCallback = OnDeckComplete;

        ExecuteMotionSequence(_current, _spawnDelay, _drawDuration, _drawEase, points);
    }

    public void PlayingEventforGraveToHands(int _current, int _last, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        tempCurrentIdx = _current;
        tempLastIdx = _last;

        currentStartCallback = null;
        currentCompleteCallback = OnGraveComplete;

        ExecuteMotionSequence(_current, _spawnDelay, _drawDuration, _drawEase, points);
    }

    public void PlayingEventforWormHole(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        tempCurrentIdx = _idx;

        currentStartCallback = OnWormholeStart;
        currentCompleteCallback = OnWormholeComplete;

        ExecuteMotionSequence(_idx, _spawnDelay, _drawDuration, _drawEase, points);
    }

    private void OnDeckStart()
    {
        poolingSystem?.CardSystem?.PlayDrawedEffect();
    }

    private void OnDeckComplete()
    {
        poolingSystem?.CardSystem?.CallOneCardDrawedBlock(tempCurrentIdx, tempLastIdx, transform.position, cardDataInstance, gameObject);
    }

    private void OnGraveComplete()
    {
        poolingSystem?.CardSystem?.CallOneCardDrawedBlock(tempCurrentIdx, tempLastIdx, transform.position, cardDataInstance, gameObject);
    }

    private void OnWormholeStart()
    {
        poolingSystem?.CardSystem?.PlayMoveToDeckMotion();
    }

    private void OnWormholeComplete()
    {
        poolingSystem?.CardSystem?.CallGraveToDeckFinished(tempCurrentIdx, gameObject);
    }

    private void ExecuteMotionSequence(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        if (activeSeq != null && activeSeq.IsActive())
            activeSeq.Kill();

        foreach (ParticleSystem vfx in particles)
            vfx?.Play(true);

        activeSeq = DOTween.Sequence();
        activeSeq.AppendInterval(_idx * _spawnDelay);

        activeSeq.Append(transform.DOLocalPath(points, _drawDuration, PathType.CubicBezier, PathMode.TopDown2D, 70, Color.green)
            .SetUpdate(false)
            .SetEase(_drawEase)
            .OnStart(OnSequenceStart)
            .OnComplete(OnSequenceComplete));
    }

    private void OnSequenceStart()
    {
        gameObject.SetActive(true);
        LoopRotate();

        currentStartCallback?.Invoke();
        currentStartCallback = null;

        onStartCallbackWithParam?.Invoke(this);
        onStartCallbackWithParam = null;
    }

    private void OnSequenceComplete()
    {
        if (activeRotate != null && activeRotate.IsActive())
            activeRotate.Kill();

        currentCompleteCallback?.Invoke();
        currentCompleteCallback = null;

        onCompleteCallbackWithParam?.Invoke(this);
        onCompleteCallbackWithParam = null;
    }

    private void LoopRotate()
    {
        if (activeRotate != null && activeRotate.IsActive())
            activeRotate.Kill();

        activeRotate = visual.DORotate(new Vector3(0f, 0f, 360f), rotateDuration, RotateMode.FastBeyond360)
            .SetUpdate(false)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}