using UnityEngine;

[ExecuteAlways]
public class UIAnchorPixelSnap : MonoBehaviour
{
    [SerializeField] private bool snapPosition = true;

    RectTransform rt;
    Canvas canvas;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        if (!rt) rt = GetComponent<RectTransform>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!rt || !canvas) return;

        float s = canvas.scaleFactor;

        if (snapPosition)
        {
            Vector2 p = rt.anchoredPosition;
            p.x = Mathf.Round(p.x * s) / s;
            p.y = Mathf.Round(p.y * s) / s;
            rt.anchoredPosition = p;
        }
    }
}
