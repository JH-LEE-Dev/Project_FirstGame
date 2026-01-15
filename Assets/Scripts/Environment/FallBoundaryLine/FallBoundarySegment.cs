using UnityEngine;

public class FallBoundarySegment : MonoBehaviour
{
    private SpriteRenderer sr;

    private float shakePosAmp = 0.03f;     // danger 상태 흔들림
    private float shakeRotAmp = 1.0f;       // danger 회전 흔들림 (각도임)
    private float noiseFreq = 18f;          // 흔들림 속도

    [Header("Warning Colors")]
    [SerializeField] private Color warningOnColor = new Color(1f, 0.35f, 0f, 1f); // ON 코랄
    [SerializeField] private Color warningOffColor = new Color(0.53f, 0.58f, 0.85f, 1f); // OFF 보라

    private int seed = 12345;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
    }

    public void SetSeed(int s)
    {
        seed = s;
    }

    public void SetTransform(Vector3 pos, Quaternion rot, float alpha, float danger01)
    {
        float t = Time.time * noiseFreq;

        // 노이즈 공식
        float nx = Mathf.PerlinNoise(seed * 0.013f, t) * 2f - 1f;
        float ny = Mathf.PerlinNoise(seed * 0.017f, t + 11.7f) * 2f - 1f;
        float nz = Mathf.PerlinNoise(seed * 0.021f, t + 23.4f) * 2f - 1f;

        Vector3 offset = new Vector3(nx, ny, 0f) * (shakePosAmp * danger01);
        float rotOffsetZ = nz * (shakeRotAmp * danger01);

        transform.position = pos + offset;
        transform.rotation = rot * Quaternion.Euler(0f, 0f, rotOffsetZ);

        if (sr != null)
        {
            Color c = Color.Lerp(warningOffColor, warningOnColor, danger01);
            c.a = alpha;
            sr.color = c;
        }
    }
}