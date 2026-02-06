using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Enemy : MonoBehaviour
{
    [Header("Main Settings")]
    private Enemy owner;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

    private void Awake()
    {

    }

    private void LateUpdate()
    {
        Update_Position();
    }

    public void Init(Enemy target)
    {
        owner = target;
    }

    public void Update_HPBar(float ratio)
    {
        if (null == hpSlider)
            return;

        hpSlider.value = Mathf.Clamp01(ratio);
    }

    public void Update_HPText(float _currentValue)
    {
        hpText.text = Mathf.RoundToInt(_currentValue).ToString();
    }

    private void Update_Position()
    {
        // 오프셋을 빼와서 추가 연산도 해야 함


    }
}
