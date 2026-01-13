using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Character_Visual : MonoBehaviour
{
    private Character owner;

    [SerializeField] private Character_Face face;


    [Header("Targets")]
    [SerializeField] private Transform body;
    [SerializeField] private Transform backRings;
    [SerializeField] private Transform frontRings;

    [Header("Time")]
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Body Hover")]
    [SerializeField] private float hoverAmplitude = 0.004f;
    [SerializeField] private float hoverDuration = 2.2f;

    [Header("Body Z Rotate")]
    [SerializeField] private float bodyRotateAmplitude = 1.5f;
    [SerializeField] private float bodyRotateDuration = 3.0f;

    [Header("Rings Z Rotate")]
    [SerializeField] private float ringRotateAmplitude = 1f;
    [SerializeField] private float ringRotateDuration = 2.6f;

    [Header("Rings Hover")]
    [SerializeField] private float ringHoverAmplitude = 0.003f;
    [SerializeField] private float ringHoverDuration = 2f;

    [Header("Blink")]
    [SerializeField] private bool enableBlink = true;   // ´« ±ôºýÀÓ
    [SerializeField] private Vector2 blinkInterval = new Vector2(3f, 6f); // ´ë±â½Ã°£
    [SerializeField] private Vector2 closeHold = new Vector2(0.09f, 0.15f); // ´« °¨´Â ½Ã°£
    [SerializeField] private Vector2 betweenDoubleBlink = new Vector2(0.07f, 0.08f); // µÎ¹ø ±ôºý »çÀÌ
    [SerializeField, Range(0f, 1f)] private float doubleBlinkChance = 0.5f; // µÎ¹ø ±ôºý È®·ü


    [Header("Flip / Rings Lean")]
    [SerializeField] private float ringLeanAngle = 26f;     // ±â¿ï±â °¢µµ(µµ)
    [SerializeField] private float ringLeanDuration = 0.75f;
    [SerializeField] private Ease ringLeanEase = Ease.OutQuint;

    private float ringLeanZ = 0f;
    private Tween ringLeanTween;
    private Dir currentDir = Dir.Left;

    // base pose
    private Vector3 bodyBasePos;
    private float bodyBaseZ;

    private Vector3 backRingBasePos;
    private float backRingBaseZ;

    private Vector3 frontRingBasePos;
    private float frontRingBaseZ;



    private Coroutine blinkCo;


    private void Awake()
    {
        if (body == null) body = transform;

        bodyBasePos = body.localPosition;
        bodyBaseZ = body.localEulerAngles.z;

        if (backRings != null)
        {
            backRingBasePos = backRings.localPosition;
            backRingBaseZ = backRings.localEulerAngles.z;
        }

        if (frontRings != null)
        {
            frontRingBasePos = frontRings.localPosition;
            frontRingBaseZ = frontRings.localEulerAngles.z;
        }

        SnapFlip(Dir.Left);
    }

    private void OnEnable()
    {
        StartBlink();
    }

    private void OnDisable()
    {
        ringLeanTween?.Kill();
        StopBlink();
    }

    private void OnDestroy()
    {
        ringLeanTween?.Kill();
        StopBlink();
    }

    public void Bind(Character character)
    {
        owner = character;


    }

    private void LateUpdate()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;

        // Body
        float hoverPhase = (t / hoverDuration) * Mathf.PI * 2f;
        float rotPhase = (t / bodyRotateDuration) * Mathf.PI * 2f;

        Vector3 bPos = bodyBasePos;
        bPos.y += Mathf.Sin(hoverPhase) * hoverAmplitude;

        float bZ = bodyBaseZ + Mathf.Sin(rotPhase) * bodyRotateAmplitude;

        body.localPosition = bPos;
        body.localRotation = Quaternion.Euler(0f, 0f, bZ);

        // Rings
        float ringRotPhase = (t / ringRotateDuration) * Mathf.PI * 2f;
        float ringHoverPhase = (t / ringHoverDuration) * Mathf.PI * 2f;

        float ringZDelta = Mathf.Sin(ringRotPhase) * ringRotateAmplitude;
        float ringYDelta = Mathf.Sin(ringHoverPhase) * ringHoverAmplitude;

        // ringLeanZ¸¦ "Ãß°¡ È¸Àü"À¸·Î ÇÕ»ê
        ApplyRing(backRings, backRingBasePos, backRingBaseZ, ringZDelta, ringYDelta, ringLeanZ);
        ApplyRing(frontRings, frontRingBasePos, frontRingBaseZ, ringZDelta, ringYDelta, ringLeanZ);

    }

    private void ApplyRing(Transform ring, Vector3 basePos, float baseZ, float zDelta, float yDelta, float leanZ)
    {
        if (ring == null) return;

        var p = basePos;
        p.y += yDelta;
        ring.localPosition = p;

        ring.localRotation = Quaternion.Euler(0f, 0f, baseZ + zDelta + leanZ);
    }


    public void SetFace(FaceExpression expr)
    {
        face?.SetExpression(expr);
    }

    private void StartBlink()
    {
        StopBlink();
        if (!enableBlink) return;

        blinkCo = StartCoroutine(BlinkLoop());
    }

    private void StopBlink()
    {
        if (blinkCo != null)
        {
            StopCoroutine(blinkCo);
            blinkCo = null;
        }

        SetFace(FaceExpression.Idle);
    }

    // Blink
    private IEnumerator BlinkLoop()
    {
        SetFace(FaceExpression.Idle);

        while (true)
        {
            float wait = Random.Range(blinkInterval.x, blinkInterval.y);
            yield return useUnscaledTime ? new WaitForSecondsRealtime(wait) : new WaitForSeconds(wait);

            bool doDouble = Random.value < doubleBlinkChance;

            // 1È¸ ±ôºý
            SetFace(FaceExpression.Close);
            float hold1 = Random.Range(closeHold.x, closeHold.y);
            yield return useUnscaledTime ? new WaitForSecondsRealtime(hold1) : new WaitForSeconds(hold1);
            SetFace(FaceExpression.Idle);

            if (doDouble)
            {
                float gap = Random.Range(betweenDoubleBlink.x, betweenDoubleBlink.y);
                yield return useUnscaledTime ? new WaitForSecondsRealtime(gap) : new WaitForSeconds(gap);

                SetFace(FaceExpression.Close);
                float hold2 = Random.Range(closeHold.x, closeHold.y);
                yield return useUnscaledTime ? new WaitForSecondsRealtime(hold2) : new WaitForSeconds(hold2);
                SetFace(FaceExpression.Idle);
            }
        }
    }

    public void Flip(Dir _dir)
    {
        currentDir = _dir;

        if (body != null)
        {
            Vector3 bs = body.localScale;
            float absX = Mathf.Abs(bs.x);

            bs.x = (_dir == Dir.Left) ? absX : -absX;
            body.localScale = bs;
        }

        float targetLean = 0f;

        if (_dir == Dir.Left) targetLean = +ringLeanAngle;
        if (_dir == Dir.Right) targetLean = -ringLeanAngle;

        ringLeanTween?.Kill();
        ringLeanTween = DOTween.To(() => ringLeanZ, v => ringLeanZ = v, targetLean, ringLeanDuration)
            .SetEase(ringLeanEase);
    }

    private void SnapFlip(Dir dir)
    {
        currentDir = dir;
        if (body != null)
        {
            Vector3 bs = body.localScale;
            float absX = Mathf.Abs(bs.x);
            bs.x = (dir == Dir.Left) ? absX : -absX;
            body.localScale = bs;
        }

        if (dir == Dir.Left) ringLeanZ = +ringLeanAngle;
        if (dir == Dir.Right) ringLeanZ = -ringLeanAngle;
    }
}
