using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar_Enemy : MonoBehaviour
{
    [Header("Main Settings")]
    //[SerializeField] private script visual; // 나중에 비주얼쪽 스크립트 물려 받으면 오프셋 받아 올 것임 
    [SerializeField] private SpriteRenderer hpSlider;
    [SerializeField] private TMP_Text hpText;

    private void Awake()
    {

    }

    public void Update_HPBar(float ratio)
    {
        
    }

    public void Update_HPText(float _currentValue)
    {
        hpText.text = Mathf.RoundToInt(_currentValue).ToString();
    }
}
