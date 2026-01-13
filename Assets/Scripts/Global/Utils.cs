using UnityEngine;

public static class UIWorldUtil
{
    // 반대쪽 좌표 변환
    public static Vector3 OverlayAnchoredToWorld_FlipX(
        RectTransform canvasRt,
        Vector2 anchoredPos,
        Camera worldCam,
        float worldZ = 0f
    )
    {
        anchoredPos.x = -anchoredPos.x;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            null,
            canvasRt.TransformPoint(anchoredPos)
        );

        var w = worldCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(worldCam.transform.position.z - worldZ)));
        w.z = worldZ;
        return w;
    }

    // Screen 좌표를 "z=worldZ 평면" 위의 월드 좌표로 변환 (카메라 이동/셰이크/원근에 안전)
    public static Vector3 ScreenToWorldOnZPlane(Camera cam, Vector2 screenPos, float worldZ)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        float dz = ray.direction.z;
        if (Mathf.Abs(dz) < 1e-6f)
        {
            Vector3 fallback = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            fallback.z = worldZ;
            return fallback;
        }

        float t = (worldZ - ray.origin.z) / dz;
        return ray.origin + ray.direction * t;
    }

    // UI(RectTransform)의 현재 화면상 위치를 월드(z=worldZ)로 변환.
    public static Vector3 OverlayUIToWorld(
        RectTransform uiRt,
        Camera worldCam,
        float worldZ = 0f
    )
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiRt.position);

        Vector3 w = ScreenToWorldOnZPlane(worldCam, screenPos, worldZ);
        w.z = worldZ;
        return w;
    }

    // UI의 특정 로컬 포인트(예: 중심, 모서리 등)를 월드로 변환하고 싶을 때.
    public static Vector3 OverlayUILocalPointToWorld(
        RectTransform uiRt,
        Vector2 localPoint,
        Camera worldCam,
        float worldZ = 0f
    )
    {
        Vector3 uiWorldPoint = uiRt.TransformPoint(localPoint);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, uiWorldPoint);

        Vector3 w = ScreenToWorldOnZPlane(worldCam, screenPos, worldZ);
        w.z = worldZ;
        return w;
    }

    // 월드 좌표를 Canvas 로컬 좌표로 변환 (Overlay 기준, pivot 원점 좌표)
    public static Vector2 WorldToOverlayCanvasLocal(
        RectTransform canvasRt,
        Vector3 worldPos,
        Camera worldCam
    )
    {
        Vector2 screenPos = worldCam.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRt,
            screenPos,
            null, // Overlay는 null
            out Vector2 canvasLocal
        );

        return canvasLocal;
    }

    // 월드 좌표를 "특정 RectTransform의 부모 기준 로컬 좌표"로 변환.
    public static Vector2 WorldToOverlayLocalInParent(
        RectTransform parentRt,
        Vector3 worldPos,
        Camera worldCam
    )
    {
        Vector2 screenPos = worldCam.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRt,
            screenPos,
            null,
            out Vector2 parentLocal
        );

        return parentLocal;
    }
}