using System.Collections;
using System.Linq;
using UnityEngine;

public class CanvasSystem : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private int unitforSorting = 5;

    public void Initialize()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
    }

    public IEnumerator InitializeChildrenCanvas()
    {
        yield return null;

        if (canvas == null)
            canvas = GetComponent<Canvas>();

        Canvas[] children = GetComponentsInChildren<Canvas>(true);
        int size = children.Count();

        for (int i = 0; i < size; ++i)
        {
            if (null == children[i] || children[i] == canvas)
                continue;

            bool wasActive = children[i].gameObject.activeSelf;
            if (!wasActive)
                children[i].gameObject.SetActive(true);

            children[i].overrideSorting = true;
            children[i].sortingLayerName = canvas.sortingLayerName;
            children[i].sortingOrder = canvas.sortingOrder + i * unitforSorting;

            if (!wasActive)
                children[i].gameObject.SetActive(false);
        }
    }
}
