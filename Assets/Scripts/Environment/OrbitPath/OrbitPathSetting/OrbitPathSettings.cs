using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Environment/OrbitSettings")]
public class OrbitPathSettings : ScriptableObject
{
    [Header("Arc Definition")]
    [SerializeField] public Transform center;                   // 구 센터
    [SerializeField] public float radius = 8.9f;                // 반지름
    [SerializeField] public float startAngleDeg = 63f;          // 시작 각도(도)
    [SerializeField] public float endAngleDeg = 117f;           // 끝 각도(도)

    [Header("Dash Settings")]
    [SerializeField] public int pathLineCount = 20;
    [SerializeField] public float flowSpeedPerSec = 0.1f;

    [Header("Ref")]
    [SerializeField] public GameObject pathLinePrefab;
}