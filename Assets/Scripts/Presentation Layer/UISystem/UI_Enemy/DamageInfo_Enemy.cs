using TMPro;
using UnityEditor;
using UnityEngine;

public class DamageInfo_Enemy : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TMP_Text damageText;

    private IEnemyData owner;
    private EnemyUI mediator;

    private void Awake()
    {
        
    }

    public void Init(IEnemyData _owner, EnemyUI _mediator)
    {
        owner = _owner;
        mediator = _mediator;

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (null == owner)
            return;

        damageText.text = Mathf.RoundToInt(owner.enemyStatProvider.attack).ToString();
    }
}
