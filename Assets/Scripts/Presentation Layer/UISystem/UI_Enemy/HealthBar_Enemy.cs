using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Enemy : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    private IEnemyData owner;
    private EnemyUI mediator;

    private float prevHealth;

    private void OnDisable()
    {
        if (null != owner)
        {
            owner.healthComponentProvider.TakeDamageEvent -= UpdateInfo;
        }
    }

    public void Init(IEnemyData _owner, EnemyUI _mediator)
    {
        owner = _owner;
        mediator = _mediator;

        if (null != owner)
        {
            UpdateInfo();
            owner.healthComponentProvider.TakeDamageEvent -= UpdateInfo;
            owner.healthComponentProvider.TakeDamageEvent += UpdateInfo;
        }
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
            mediator.ReturnObject();
        }
    }
}
