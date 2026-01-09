using System.Linq;
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

    public void InitializeChildrenCanvas()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        Canvas[] children = GetComponentsInChildren<Canvas>(true);
        int size = children.Count();

        for (int i = 0; i < size; ++i)
        {
            if (null == children[i] || children[i] == canvas)
                continue;

            children[i].overrideSorting = true;
            children[i].sortingOrder = canvas.sortingOrder + i;
        }
    }
}
