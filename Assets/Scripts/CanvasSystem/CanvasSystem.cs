using UnityEngine;

public class CanvasSystem : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        Debug.Log("Awake!");
    }
}
