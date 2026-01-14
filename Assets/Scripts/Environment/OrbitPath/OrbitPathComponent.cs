using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class OrbitPathComponent : MonoBehaviour
{
    [Header("OrbitSettings")]
    [SerializeField] private OrbitPathSettings orbitPathSettings;
    [Space]
    [Header("Arc Definition")]
    [SerializeField] public Transform center;                   // 구 센터
    [SerializeField] public float radius = 8.9f;                // 반지름
    [SerializeField] public float startAngleDeg = 63f;          // 시작 각도(도)
    [SerializeField] public float endAngleDeg = 117f;           // 끝 각도(도)
    [Space]
    [Header("Dash Settings")]
    [SerializeField] private int pathLineCount = 20;
    [SerializeField] private float flowSpeedPerSec = 0.1f;
    [Space]
    [Header("Ref")]
    [SerializeField] private GameObject pathLinePrefab;
    private readonly List<OrbitPathSegment> pathLines = new();

    [Header("Alpha Settings")]
    [SerializeField] private float alphaFadeDuration = 0.25f;
    [SerializeField] private float globalAlpha = 1f;
    private Tween alphaTween;


    // 전체 흐름 제어 변수
    private float globalT;


    private void Awake()
    {
        Initialize();

        if (!center) center = transform;

        for (int i = 0; i < pathLineCount; i++)
        {
            var go = Instantiate(pathLinePrefab, center);
            var line = go.GetComponent<OrbitPathSegment>();
            if (!line) line = go.AddComponent<OrbitPathSegment>();
            pathLines.Add(line);
        }
    }

    public void Initialize()
    {
        if(orbitPathSettings == null)
        {
            Debug.LogWarning("OrbitPathComponent::Initialize -> orbitSettings is null!!");
            return;
        }

        center = orbitPathSettings.center;
        radius = orbitPathSettings.radius;
        startAngleDeg = orbitPathSettings.startAngleDeg;
        endAngleDeg = orbitPathSettings.endAngleDeg;
        pathLineCount = orbitPathSettings.pathLineCount;
        flowSpeedPerSec = orbitPathSettings.flowSpeedPerSec;
        pathLinePrefab = orbitPathSettings.pathLinePrefab;
    }

    private void Update()
    {
        DrawPathLine();
    }

    // 라인의 위치와 회전 상태를 관리할 것임.
    private void DrawPathLine()
    {

        float segmentLen = 1f / pathLineCount;

        globalT = Mathf.Repeat(globalT + flowSpeedPerSec * Time.deltaTime, segmentLen);

        int last = pathLines.Count - 1;

        for (int i = 0; i < pathLines.Count; i++)
        {
            float baseT = i * segmentLen;
            float t01 = baseT + globalT;

            Vector3 pos = PointOnArc(t01);
            Quaternion rot = RotationOnArcTangent(t01);

            float local01 = Mathf.Clamp01((t01 - baseT) / segmentLen);

            float alpha = 1f;

            // Left 3
            if (i == 0) alpha = Mathf.Lerp(0f, 0.33f, local01);
            else if (i == 1) alpha = Mathf.Lerp(0.33f, 0.66f, local01);
            else if (i == 2) alpha = Mathf.Lerp(0.66f, 1f, local01);

            // Right 3
            else if (i == last - 2) alpha = Mathf.Lerp(1f, 0.66f, local01);
            else if (i == last - 1) alpha = Mathf.Lerp(0.66f, 0.33f, local01);
            else if (i == last) alpha = Mathf.Lerp(0.33f, 0f, local01);

            pathLines[i].SetTransform(pos, rot, alpha * globalAlpha);
        }
    }

    private Vector3 PointOnArc(float t01)
    {
        float ang = Mathf.Lerp(startAngleDeg, endAngleDeg, Mathf.Clamp01(t01));
        float rad = ang * Mathf.Deg2Rad;

        Vector3 offset = (Vector3.right * Mathf.Cos(rad) + Vector3.up * Mathf.Sin(rad)) * radius;
        return center.position + offset;
    }

    // 접선 방향 회전
    private Quaternion RotationOnArcTangent(float t01)
    {
        float ang = Mathf.Lerp(startAngleDeg, endAngleDeg, Mathf.Clamp01(t01));

        // 진행 방향(end-start)에 따라 접선 보정 부호
        float dirSign = Mathf.Sign(endAngleDeg - startAngleDeg);
        float tangentAdd = (dirSign >= 0f) ? 90f : -90f;

        float zRot = ang + tangentAdd;
        return Quaternion.Euler(0f, 0f, zRot);
    }

    // 0~1값을 넣으면 정규화된 호 위의 위치를 반환함. 메인 캐릭터 움직임에 사용할 것임.
    public Vector3 GetPathPosition(float value) => PointOnArc(value);

    public void SetPathActive(bool value)
    {
        float targetAlpha = value ? 1f : 0f;

        // 기존 트윈이 있으면 중단
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
