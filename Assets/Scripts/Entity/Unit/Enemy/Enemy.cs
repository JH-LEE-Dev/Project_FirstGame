using UnityEngine;

public class Enemy : Unit
{
    private Vector2 targetPoint;

    public override void Initialize(InputManager _inputManager, WaveManager _waveManager,
        EnemyTypeData _enemyTypeData)
    {
        base.Initialize(_inputManager, _waveManager);

        waveManager.StartMoveEvent += OnMove;

        sr.sprite = _enemyTypeData.sprite;

        float scale = _enemyTypeData.scale;
        float scaleDelta = UnityEngine.Random.Range(0f, 1f);
        transform.localScale = new Vector3(scaleDelta + scale, scaleDelta + scale, 1f);
    }

    public override void ApplyDamage(float damage)
    {
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnMove()
    {
        moveComponent.ApplyImpulse();
    }

    protected override void OnDestroy()
    {
        waveManager.StartMoveEvent -= OnMove;   
    }

    public void SetTargetPoint(Vector2 _targetPoint)
    {
        targetPoint = _targetPoint;
        Vector2 targetDir = targetPoint - (Vector2)transform.position;
        targetDir.Normalize();

        moveComponent.SetMoveDirection(targetDir);
    }
}
