using UnityEngine;

[ExecuteAlways]
public class UIPixelSnap : MonoBehaviour
{
    public bool snapPosition = true;
    public bool snapSize = true;

    RectTransform rt;
    Canvas canvas;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        Debug.Log($"scaleFactor = {GetComponentInParent<Canvas>().scaleFactor}");

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

        if (snapSize)
        {
            Vector2 sz = rt.sizeDelta;
            sz.x = Mathf.Round(sz.x * s) / s;
            sz.y = Mathf.Round(sz.y * s) / s;
            rt.sizeDelta = sz;
        }
    }
}