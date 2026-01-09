using UnityEngine;

public class CanvasSystem : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    public void Initialize()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }
}
