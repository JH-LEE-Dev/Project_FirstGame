using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class FallBoundaryComponent : MonoBehaviour
{
    [Header("Arc Definition")]
    [SerializeField] public Transform center;                   // 구 센터
    [SerializeField] public float radius = 11.3f;                // 반지름
    [SerializeField] public float startAngleDeg = 63f;          // 시작 각도(도)
    [SerializeField] public float endAngleDeg = 117f;           // 끝 각도(도)

    [Header("Line Settings")]
    [SerializeField] private int LineCount = 40;
    [SerializeField] private float LineSpeedPerSec = 0.05f;

    [Header("Ref")]
    [SerializeField] private GameObject fallBoundaryPrefab;
    private readonly List<FallBoundarySegment> fallBoundarySegments = new();

    [Header("Color Settings")]
    [SerializeField] private float alphaFadeDuration = 0.25f;
    [SerializeField] private float globalAlpha = 0.35f;

    private Tween alphaTween;

    // 전체 흐름 제어 변수
    private float globalT;

    private void Awake()
    {
        if (!center) center = transform;

        for (int i = 0; i < LineCount; i++)
        {
            var go = Instantiate(fallBoundaryPrefab, center);
            var line = go.GetComponent<FallBoundarySegment>();
            if (!line) line = go.AddComponent<FallBoundarySegment>();
            fallBoundarySegments.Add(line);
        }
    }

    private void Update()
    {
        DrawPathLine();
    }

    private void DrawPathLine()
    {

        float segmentLen = 1f / LineCount;

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

            fallBoundarySegments[i].SetTransform(pos, rot, alpha * globalAlpha);
        }
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

        // 진행 방향(end-start)에 따라 접선 보정 부호
        float dirSign = Mathf.Sign(endAngleDeg - startAngleDeg);
        float tangentAdd = (dirSign >= 0f) ? 90f : -90f;

        float zRot = ang + tangentAdd;
        return Quaternion.Euler(0f, 0f, zRot);
    }

    public void SetPathActive(bool value)
    {
        float targetAlpha = value ? 0.35f : 0.03f;

        alphaTween?.Kill();

        alphaTween = DOTween
            .To(
                () => globalAlpha,
                x => globalAlpha = x,
                targetAlpha,
                alphaFadeDuration
            )
            .SetEase(Ease.Linear);
    }

}
