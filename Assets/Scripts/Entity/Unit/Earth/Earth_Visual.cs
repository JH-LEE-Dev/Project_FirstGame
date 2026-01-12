using UnityEngine;

public class Earth_Visual : MonoBehaviour
{
    [Header("Rotate")]
    [SerializeField] private float degreesPerSecond = 1f;

    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.003f;
    [SerializeField] private float floatSpeed = 1.2f;

    private RectTransform rt;
    private Vector2 baseAnchoredPos;
    private float timeOffset;

    private void Awake()
    {
        rt = (RectTransform)transform;
        baseAnchoredPos = rt.anchoredPosition;

        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }


    private void Update()
    {
        // 회전
        rt.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);

        // Floating (Y축만)
        float yOffset = Mathf.Sin(Time.time * floatSpeed + timeOffset) * floatAmplitude;
        rt.anchoredPosition = baseAnchoredPos + Vector2.up * yOffset;
    }

}
