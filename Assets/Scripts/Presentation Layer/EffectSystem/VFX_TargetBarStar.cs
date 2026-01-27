using DG.Tweening;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

using Random = UnityEngine.Random;

public class VFX_TargetBarStar : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private RectTransform visualRect;
    [SerializeField] private float totalDuration = 1f;
    [SerializeField, Range(0f, 1000f)] private float dragPower;
    [SerializeField] private Ease ease = Ease.Linear;

    private RectTransform mainRect;
    private ParticleSystem[] particles;
    private Sequence seq;

    private Vector3[] path = new Vector3[3];

    private Action<VFX_TargetBarStar> callbackEvent;

    public float savedCurrentProgress { get; private set; }
    public int savedCurrentKillCnt { get; private set; }
    public int savedEnemyMaxCnt { get; private set; }

    private void Awake()
    {
        mainRect = GetComponent<RectTransform>();
        particles = GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem vfx in particles)
        {
            if (vfx)
            {
                var main = vfx.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Custom;
                main.customSimulationSpace = GetComponentInParent<Canvas>().transform;
            }
        }
    }

    private void OnDisable()
    {
        visualRect?.DOKill();
        seq?.Kill();
    }

    public bool CheckAliveParticle()
    {
        int vfxCnt = particles.Count();
        for (int i = 0; i < vfxCnt; ++i)
        {
            bool checker = particles[i].IsAlive();

            if (false == checker)
                continue;

            return checker;
        }

        return false;
    }

    public void SetupSavedData(float _currentProgress, int _currentKillCnt, int _enemyMaxCnt)
    {
        savedCurrentProgress = _currentProgress;
        savedCurrentKillCnt = _currentKillCnt;
        savedEnemyMaxCnt = _enemyMaxCnt;
    }

    public void Play(Vector2 finalAnchoredPos, Action<VFX_TargetBarStar> callback = null)
    {
        if (null == mainRect)
            return;

        GoToTarget(finalAnchoredPos, callback);
    }

    private void GoToTarget(Vector2 finalPos, Action<VFX_TargetBarStar> callback)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        callbackEvent = callback;

        seq = DOTween.Sequence();

        Vector2 startPos = mainRect.anchoredPosition;

        Vector2 pos25 = startPos + (finalPos - startPos) * 0.25f;
        Vector2 pos75 = startPos + (finalPos - startPos) * 0.75f;

        Vector2 direction = (finalPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float randomPower = Random.Range(-1f, 1f) * dragPower;

        pos25 += perpendicular * randomPower;
        pos75 += perpendicular * randomPower;

        path[0] = finalPos;
        path[1] = pos25;
        path[2] = pos75;

        seq.Append(mainRect.DOLocalPath(path, totalDuration, PathType.CubicBezier, PathMode.TopDown2D, 10, Color.red)
            .SetEase(ease));

        seq.SetUpdate(false);
        seq.OnStart(GoToTargetStartEvent);
        seq.OnComplete(GoToTargetCompleteEvent);
    }

    private void Rotate_Infinity()
    {
        if (null == visualRect)
            return;

        visualRect.DOKill();

        visualRect.DORotate(new Vector3(0f, 0f, 360f), 3f, RotateMode.FastBeyond360)
            .SetUpdate(false)
            .SetLoops(-1);
    }

    private void GoToTargetStartEvent()
    {
        foreach (ParticleSystem vfx in particles)
            vfx?.Play(true);

        Rotate_Infinity();
        visualRect.gameObject.SetActive(true);
    }

    private void GoToTargetCompleteEvent()
    {
        foreach (ParticleSystem vfx in particles)
            vfx?.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        callbackEvent?.Invoke(this);
        callbackEvent = null;
    }
}
