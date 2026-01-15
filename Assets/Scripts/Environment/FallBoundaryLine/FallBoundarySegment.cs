using DG.Tweening;
using UnityEngine;

public class FallBoundarySegment : MonoBehaviour
{
    private SpriteRenderer sr;

    private float warningColor = 0.75f;

    [Header("Shake")]
    [SerializeField] private float shakePosAmp = 0.02f;   // 엄청 작게
    [SerializeField] private float shakeRotAmp = 1.0f;    // 도 단위(아주 작게)
    [SerializeField] private float shakeStep = 0.06f;     // 한 번 부들 간격

    [Header("Warning Color")]
    [SerializeField] private float warnTweenTime = 0.3f;
    [SerializeField] private float warningOnTarget = 0.0f;
    [SerializeField] private float warningOffTarget = 0.75f;

    private Sequence shakeSeq;
    private Tween warnTween;

    private Vector3 baseLocalPos;
    private Quaternion baseLocalRot;

    // 트리거용 함수
    public void SetAction(bool _action)
    {
        if (_action) OnAction();
        else OffAction();
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetTransform(Vector3 pos, Quaternion rot, float alpha)
    {
        transform.position = pos;
        transform.rotation = rot;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            c.g = warningColor;
            sr.color = c;
        }
    }

    private void OnAction()
    {
        // DoTween 사용가능
        // 1. 엄청 작은 량으로 부들부들되기 시작해야함. Loop
        // 2. warningColor가 0.3초안에 0으로 변해야함.
    }

    private void OffAction()
    {
        // DoTween 사용가능
        // 1. 부들부들이 멈추고 다시 원래대로 돌아가야함.
        // 2. warningColor가 0.3초안에 0.75로 변해야함.
    }
}
