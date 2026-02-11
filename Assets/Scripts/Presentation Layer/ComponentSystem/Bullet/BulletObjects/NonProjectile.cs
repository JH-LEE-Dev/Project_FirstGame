using System;
using UnityEngine;

public class NonProjectile : MonoBehaviour
{
    /// <summary>
    /// 시스템 속성 존 ----------------------------------------------
    /// </summary>

    //내부 의존성
    public BulletVisualEffectComponent effectComponent { get; private set; }

    [SerializeField] public CircleCollider2D circleCollider;
    [SerializeField] public CircleCollider2D explosionRangeCollider;
    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask outOfRangeMask;
    public SpriteRenderer sr { get; private set; }


    /// <summary>
    /// 구현 속성 존 --------------------------------------------------------
    /// </summary>

    public Vector2 prevPosition { get; private set; }
    public float range { get; private set; }

    /// <summary>
    /// 시스템 코드 존 --------------------------------------------------------
    /// </summary>

    private void Awake()
    {

    }

    public void Initialize()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        effectComponent = GetComponentInChildren<BulletVisualEffectComponent>();

        circleCollider.enabled = false;
        explosionRangeCollider.enabled = false;
        range = explosionRangeCollider.radius;
    }

    private void OnDestroy()
    {

    }

    private void Update()
    {

    }


    /// <summary>
    /// 구현 코드 존 --------------------------------------------------------
    /// </summary>
}
