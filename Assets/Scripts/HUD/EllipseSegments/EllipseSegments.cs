using UnityEngine;

public class EllipseSegments : MonoBehaviour
{
    public GameObject segmentPrefab;
    public int segmentCount = 24;

    public float radiusX = 5f;
    public float radiusY = 3f;

    public float segmentLength = 0.5f;

    void Start()
    {
        SettingEllipse();
    }

    [ContextMenu("Ellipse Setting Test")]
    public void SettingEllipse()
    {
        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / segmentCount * Mathf.PI;

            // 위치
            float x = Mathf.Cos(t) * radiusX;
            float y = Mathf.Sin(t) * radiusY;
            Vector3 pos = new Vector3(x, y, 0);

            // 방향 (타원 접선)
            Vector2 tangent = new Vector2(
                -Mathf.Sin(t) * radiusX,
                 Mathf.Cos(t) * radiusY
            ).normalized;

            GameObject seg = Instantiate(segmentPrefab, transform);
            seg.transform.localPosition = pos;

            // 회전
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            seg.transform.localRotation = Quaternion.Euler(0, 0, angle);

            // 길이 조절
            seg.transform.localScale = new Vector3(segmentLength, 1f, 1f);
        }
    }
}
