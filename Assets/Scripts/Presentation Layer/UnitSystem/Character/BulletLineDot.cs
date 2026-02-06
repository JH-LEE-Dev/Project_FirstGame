using UnityEngine;

public class BulletLineDot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    public void Init(Sprite sprite)
    {
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>(true);
        if (sr) sr.sprite = sprite;
    }
    public void SetVisible(bool v)
    {
        gameObject.SetActive(v);
    }

    public void SetAlpha(float a)
    {
        if (!sr) return;
        var c = sr.color;
        c.a = Mathf.Clamp01(a);
        sr.color = c;
    }
}
