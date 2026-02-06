using System;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float tempOffset;
    [SerializeField] private HealthBar_Enemy healthBar;
    [SerializeField] private DamageInfo_Enemy damageInfo;

    private RectTransform topRect;

    private IEnemyData owner;
    private Action<GameObject> returnEvent;

    private void Awake()
    {
        if (null == topRect)
            topRect = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        Update_Position();
    }

    private void OnDisable()
    {
        returnEvent = null;

        if (null != owner)
        {
            owner.EnemyIsDeadEvent -= ReturnObject;
        }
    }

    public void Init(IEnemyData target, Action<GameObject> _returnEvent)
    {
        owner = target;

        returnEvent -= _returnEvent;
        returnEvent += _returnEvent;

        if (null != owner)
        {
            owner.EnemyIsDeadEvent -= ReturnObject;
            owner.EnemyIsDeadEvent += ReturnObject;
        }

        if (null != healthBar)
        {
            healthBar.Init(target, this);
        }

        if (null != damageInfo)
        {
            damageInfo.Init(target, this);
        }
    }

    public void ReturnObject() => returnEvent?.Invoke(this.gameObject);

    private void Update_Position()
    {
        if (null == owner)
            return;

        // 오프셋을 빼와서 추가 연산도 해야 함

        Vector3 finalPos = owner.GetTransform().position;
        finalPos.y += tempOffset;

        topRect.anchoredPosition = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(finalPos, topRect);
    }
}
