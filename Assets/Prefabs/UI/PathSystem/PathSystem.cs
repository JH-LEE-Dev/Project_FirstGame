using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum DragDir
{
    UP,
    DOWN,
    RANDOM,
}

public class PathSystem : MonoBehaviour
{
    public Vector3[] GetDragPath(GameObject performer, Vector3 _start, Vector3 _target, float _dragPower, DragDir _dir = DragDir.RANDOM)
    {
        if (null == performer)
            return null;

        RectTransform parentRect = performer.GetComponent<RectTransform>();
        if (null == parentRect)
            return null;

        Vector3 startPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(_start, parentRect);
        Vector3 targetPos = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(_target, parentRect);

        Vector2 pos25 = startPos + (targetPos - startPos) * 0.25f;
        Vector2 pos75 = startPos + (targetPos - startPos) * 0.75f;

        Vector2 direction = (targetPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        if (DragDir.DOWN == _dir)
        {
            pos25 += perpendicular * Random.Range(0.85f, 1f) * _dragPower;
            pos75 += perpendicular * Random.Range(0.85f, 1f) * _dragPower;
        }
        else if (DragDir.UP == _dir)
        {
            pos25 += perpendicular * Random.Range(-0.85f, -0.1f) * _dragPower;
            pos75 += perpendicular * Random.Range(-0.85f, -0.1f) * _dragPower;
        }
        else
        {
            pos25 += perpendicular * Random.Range(-1f, 1f) * _dragPower;
            pos75 += perpendicular * Random.Range(-1f, 1f) * _dragPower;
        }

        Vector3[] path = { targetPos, pos25, pos75 };

        return path;
    }
}
