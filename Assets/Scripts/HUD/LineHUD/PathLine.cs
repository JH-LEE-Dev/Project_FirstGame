using UnityEngine;

public class PathLine : MonoBehaviour
{

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    public void SetTransform(Vector3 pos, Quaternion rot, float alpha)
    {
        transform.position = pos;
        transform.rotation = rot;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}
