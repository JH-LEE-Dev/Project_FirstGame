using UnityEngine;

public class EStatComponent : StatComponent,IEnemyStatProvider
{
    public float attack { get; private set; }
    private float initialAttack;

    public void Initialize(float _attack)
    {
        initialAttack = _attack;

        attack = initialAttack;
    }
}
