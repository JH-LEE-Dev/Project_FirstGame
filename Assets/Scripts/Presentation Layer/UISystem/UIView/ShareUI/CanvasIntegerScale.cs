using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CanvasIntegerScale : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(640, 360);
    public bool useMinAxis = true;
    public int minScale = 1;
    public int maxScale = 8;

    CanvasScaler scaler;

    void OnEnable()
    {
        scaler = GetComponent<CanvasScaler>();
        Apply();
    }

    void Update() => Apply();

    void Apply()
    {
        if (!scaler) return;

        float sx = Screen.width / referenceResolution.x;
        float sy = Screen.height / referenceResolution.y;
        float s = useMinAxis ? Mathf.Min(sx, sy) : Mathf.Max(sx, sy);

        int intScale = Mathf.Clamp(Mathf.FloorToInt(s), minScale, maxScale);
        if (intScale < 1) intScale = 1;

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = intScale;
        scaler.referencePixelsPerUnit = 100;
    }
}
