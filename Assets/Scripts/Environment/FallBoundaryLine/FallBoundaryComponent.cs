using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FallBoundaryComponent : MonoBehaviour
{
    [Header("Arc Definition")]
    [SerializeField] public Transform center;                       // 구 센터
    [SerializeField] public float radius = 11f;                   // 반지름
    [SerializeField] public float startAngleDeg = 63f;              // 시작 각도(도)
    [SerializeField] public float endAngleDeg = 117f;               // 끝 각도(도)

    [Header("Line Settings")]
    [SerializeField] private int LineCount = 40;
    [SerializeField] private float LineSpeedPerSec = 0.05f;

    [Header("Ref")]
    [SerializeField] private GameObject fallBoundaryPrefab;
    private readonly List<FallBoundarySegment> fallBoundarySegments = new();

    [Header("Color Settings")]
    [SerializeField] private float alphaFadeDuration = 0.25f;
    [SerializeField] private float globalAlpha = 0.15f;
    [SerializeField] private float globalMinAlpha = 0.02f;
    [SerializeField] private float globalMaxAlpha = 0.15f;

    private Tween alphaTween;

    // 전체 흐름 제어 변수
    private float globalT;


    // Danger (TEST)
    [Header("Danger (TEST)")]
    [SerializeField] private List<Transform> testMonsters = new();
    [SerializeField] private float dangerNear = 0.8f;
    [SerializeField] private float dangerFar = 3f;

    [Tooltip("위험도 변화 부드럽게")]
    [SerializeField] private float dangerSmoothTime = 0.15f;

    // 세그먼트마다 위험도 스무딩용
    private float[] dangerSmoothed;
    private float[] dangerVel;

    ///

    private void Awake()
    {
        if (!center) center = transform;

        dangerSmoothed = new float[LineCount];
        dangerVel = new float[LineCount];

        for (int i = 0; i < LineCount; i++)
        {
            var go = Instantiate(fallBoundaryPrefab, center);
            var seg = go.GetComponent<FallBoundarySegment>();
            if (!seg) seg = go.AddComponent<FallBoundarySegment>();

            seg.SetSeed(i * 97 + 13);

            fallBoundarySegments.Add(seg);
        }

    }

    private void Update()
    {
        DrawPathLine();
    }

    private void DrawPathLine()
    {
        float segmentLen = 1f / LineCount;

        // 기존 눈속임 유지
        globalT = Mathf.Repeat(globalT + LineSpeedPerSec * Time.deltaTime, segmentLen);

        int last = fallBoundarySegments.Count - 1;

        for (int i = 0; i < fallBoundarySegments.Count; i++)
        {
            float baseT = i * segmentLen;
            float t01 = baseT + globalT;

            Vector3 pos = PointOnArc(t01);
            Quaternion rot = RotationOnArcTangent(t01);

            float local01 = Mathf.Clamp01((t01 - baseT) / segmentLen);

            float alpha = 1f;

            // Left 4
            if (i == 0) alpha = Mathf.Lerp(0f, 0.25f, local01);
            else if (i == 1) alpha = Mathf.Lerp(0.25f, 0.5f, local01);
            else if (i == 2) alpha = Mathf.Lerp(0.5f, 0.75f, local01);
            else if (i == 3) alpha = Mathf.Lerp(0.75f, 1f, local01);

            // Right 4
            else if (i == last - 3) alpha = Mathf.Lerp(1f, 0.75f, local01);
            else if (i == last - 2) alpha = Mathf.Lerp(0.75f, 0.5f, local01);
            else if (i == last - 1) alpha = Mathf.Lerp(0.5f, 0.25f, local01);
            else if (i == last) alpha = Mathf.Lerp(0.25f, 0f, local01);

            // danger
            float dangerRaw = ComputeDanger01(pos);
            float danger = Mathf.SmoothDamp(
                dangerSmoothed[i],
                dangerRaw,
                ref dangerVel[i],
                dangerSmoothTime
            );
            dangerSmoothed[i] = danger;

            fallBoundarySegments[i].SetTransform(pos, rot, alpha * globalAlpha, danger);
        }
    }

    private float ComputeDanger01(Vector3 worldPos)
    {
        if (testMonsters == null || testMonsters.Count == 0)
            return 0f;

        float minDist = float.MaxValue;

        for (int i = 0; i < testMonsters.Count; i++)
        {
            var t = testMonsters[i];
            if (!t) continue;

            float d = Vector3.Distance(worldPos, t.position);
            if (d < minDist) minDist = d;
        }

        if (minDist == float.MaxValue)
            return 0f;

        float x = Mathf.InverseLerp(dangerFar, dangerNear, minDist); // far->0, near->1
        return Mathf.SmoothStep(0f, 1f, x);
    }

    private Vector3 PointOnArc(float t01)
    {
        float ang = Mathf.Lerp(startAngleDeg, endAngleDeg, Mathf.Clamp01(t01));
        float rad = ang * Mathf.Deg2Rad;

        Vector3 offset = (Vector3.right * Mathf.Cos(rad) + Vector3.up * Mathf.Sin(rad)) * radius;
        return center.position + offset;
    }

    private Quaternion RotationOnArcTangent(float t01)
    {
        float ang = Mathf.Lerp(startAngleDeg, endAngleDeg, Mathf.Clamp01(t01));

        float dirSign = Mathf.Sign(endAngleDeg - startAngleDeg);
        float tangentAdd = (dirSign >= 0f) ? 90f : -90f;

        float zRot = ang + tangentAdd;
        return Quaternion.Euler(0f, 0f, zRot);
    }

    public void SetPathActive(bool value)
    {
        float targetAlpha = value ? globalMaxAlpha : globalMinAlpha;

        alphaTween?.Kill();
        alphaTween = DOTween
            .To(() => globalAlpha, x => globalAlpha = x, targetAlpha, alphaFadeDuration)
            .SetEase(Ease.Linear);
    }
}
