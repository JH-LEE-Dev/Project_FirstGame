using UnityEngine;

public class Earth_Visual : MonoBehaviour
{
    [Header("Rotate")]
    [SerializeField] private float degreesPerSecond = 1f;

    [Header("Floating")]
    [SerializeField] private float floatAmplitude = 0.03f;
    [SerializeField] private float floatSpeed = 1.5f;

    private Vector2 basePos;
    private float timeOffset;

    private void Awake()
    {
        basePos = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }


    private void Update()
    {
        // 회전
        transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);

        // Floating (Y축만)
        float yOffset = Mathf.Sin(Time.time * floatSpeed + timeOffset) * floatAmplitude;
        transform.position = basePos + Vector2.up * yOffset;
    }

}
