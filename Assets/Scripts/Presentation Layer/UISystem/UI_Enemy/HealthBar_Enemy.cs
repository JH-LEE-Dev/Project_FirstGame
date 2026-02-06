using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Enemy : MonoBehaviour
{
    [Header("Main Settings")]
    private IEnemyData owner;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private float tempOffset = 0.5f;

    private float prevHealth;

    private RectTransform topRect;

    private event Action<GameObject> returnEvent;

    private void Awake()
    {
        if (null == topRect)
        {
            topRect = GetComponent<RectTransform>();
        }
        
    }

    private void LateUpdate()
    {
        Update_Position();
    }

    private void OnDisable()
    {
        if (null != owner)
        {
            owner.healthComponentProvider.TakeDamageEvent -= UpdateInfo;
        }

        returnEvent = null;
    }

    public void Init(IEnemyData target, Action<GameObject> _returnEvent)
    {
        owner = target;

        if (null != owner)
        {
            UpdateInfo();
            owner.healthComponentProvider.TakeDamageEvent -= UpdateInfo;
            owner.healthComponentProvider.TakeDamageEvent += UpdateInfo;
        }

        returnEvent -= _returnEvent;
        returnEvent += _returnEvent;
    }

    private void Update_HPBar(float ratio)
    {
        if (null == hpSlider)
            return;

        hpSlider.value = Mathf.Clamp01(ratio);
    }

    private void Update_HPText(float _currentValue)
    {
        hpText.text = Mathf.RoundToInt(_currentValue).ToString();
    }

    private void Update_Position()
    {
        if (null == owner)
            return;

        // 오프셋을 빼와서 추가 연산도 해야 함

        Vector3 finalPos = owner.GetTransform().position;
        finalPos.y += tempOffset;

        topRect.anchoredPosition = UIWorldUtil.GetGenerateTheAnchoredPosfromWorldPos(finalPos, topRect);
    }

    public void UpdateInfo()
    {
        if (null == owner || null == hpSlider || null == hpText)
            return;

        float maxHealth = owner.GetMaxHealth();
        float currentHealth = owner.GetCurrentHealth();

        float healthRatio = currentHealth / maxHealth;

        // 추후 연출 도입
        Update_HPBar(healthRatio);
        Update_HPText(currentHealth);

        if (0 >= currentHealth)
        {
            returnEvent.Invoke(this.gameObject);
        }
    }
}
