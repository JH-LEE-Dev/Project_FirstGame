using UnityEngine;
using System;
using System.Collections.Generic;


public enum FaceExpression
{
    Idle,
    Close,
    Angry,
}

[Serializable]
public struct FaceEntry
{
    public FaceExpression expr;
    public Sprite sprite;
}


public class Character_Face : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private FaceEntry[] spriteEntrys;

    private Dictionary<FaceExpression, Sprite> map;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        map = new Dictionary<FaceExpression, Sprite>(spriteEntrys.Length);
        foreach (var e in spriteEntrys)
        {
            if (e.sprite != null)
                map[e.expr] = e.sprite;
        }
    }

    public void SetExpression(FaceExpression expr)
    {
        if (sr == null) return;

        if (map != null && map.TryGetValue(expr, out var sp) && sp != null)
            sr.sprite = sp;
    }
}
