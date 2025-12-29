using UnityEngine;

public class Character : Unit
{
    public override void Initialize(InputManager _inputManager, WaveManager _waveManager = null, 
        EnemyTypeData _enemyTypeData = null)
    {
        base.Initialize(_inputManager, _waveManager);

        inputManager.inputReader.MoveEvent += OnMove;
    }

    public override void ApplyDamage(float damage)
    {

    }

    protected override void Update()
    {
        base.Update();


    }

    protected override void OnDestroy()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
    }
}
