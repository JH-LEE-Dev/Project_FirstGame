using DG.Tweening;
using System;
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
    private ParticleSystem particle;
    private TrailRenderer trail;
    private Sequence seq;

    private void Awake()
    {
        mainRect = GetComponent<RectTransform>();
        particle = GetComponentInChildren<ParticleSystem>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnDisable()
    {
        visualRect?.DOKill();
        seq?.Kill();
        trail.Clear();
    }

    public bool CheckAliveParticle()
    {
        if (null == particle)
            return false;

        return particle.IsAlive(true);
    }

    public void Play(Vector2 finalAnchoredPos, Action callback = null)
    {
        if (null == mainRect)
            return;

        GoToTarget(finalAnchoredPos, callback);
    }

    private void GoToTarget(Vector2 finalPos, Action callback)
    {
        if (null != seq && seq.IsActive())
            seq.Kill();

        seq = DOTween.Sequence();

        Vector2 startPos = mainRect.anchoredPosition;

        Vector2 pos25 = startPos + (finalPos - startPos) * 0.25f;
        Vector2 pos75 = startPos + (finalPos - startPos) * 0.75f;

        Vector2 direction = (finalPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        float randomPower = Random.Range(-1f, 1f) * dragPower;

        pos25 += perpendicular * randomPower;
        pos75 += perpendicular * randomPower;

        Vector3[] path = { finalPos, pos25, pos75 };

        seq.Append(mainRect.DOLocalPath(path, totalDuration, PathType.CubicBezier, PathMode.TopDown2D, 10, Color.red)
            .SetEase(ease));

        seq.SetUpdate(false);

        seq.OnStart(() =>
        {
            particle?.Play(true);
            trail.Clear();

            Rotate_Infinity();
            visualRect.gameObject.SetActive(true);
        });

        seq.OnComplete(() =>
        {
            particle?.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            callback?.Invoke();
        });
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
}
