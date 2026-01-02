using UnityEngine;

public class Enemy : Unit
{
    private Vector2 targetPoint;
    [SerializeField] private LayerMask gravityLayerMask;
    private bool bAccelerate = false;

    private TrailRenderer trailRenderer;

    public override void Initialize(InputManager _inputManager, GameServiceLocator _gameServiceLocator,
        WaveManager _waveManager,EnemyTypeData _enemyTypeData)
    {
        base.Initialize(_inputManager, _gameServiceLocator, _waveManager);

        waveManager.StartMoveEvent += OnMove;

        sr.sprite = _enemyTypeData.sprite;

        float scale = _enemyTypeData.scale;
        float scaleDelta = UnityEngine.Random.Range(0f, 1f);
        transform.localScale = new Vector3(scaleDelta + scale, scaleDelta + scale, 1f);
        moveComponent.SetImpulsePower(_enemyTypeData.moveForce);

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.material=sr.material;
        trailRenderer.material.mainTexture = sr.sprite.texture;
        Color c = trailRenderer.material.color;
        c.a = 0.3f;
        trailRenderer.material.color = c;

        healthComponent.SetHealth(_enemyTypeData.health);
    }

    public override void TakeDamage(float damage)
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

    public void ApplyImpulse()
    {
        moveComponent.ApplyImpulse();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger) 
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Earth"))
        {
            effectComponent.PlayExplosionEffect();

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            col.enabled = false;
            sr.enabled = false;

            InvokeUnitIsDead();
            gameServiceLocator.PlayCameraShake();

            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Gravity"))
        {
            bAccelerate = true;
            moveComponent.SetAccelerate(bAccelerate);
            ResetDamping();

            return;
        }
    }
}
