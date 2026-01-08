using UnityEngine;

public class Enemy : Unit
{
    [SerializeField] private LayerMask gravityLayerMask;

    private Vector2 targetPoint;
    private bool bAccelerate = false;
    private EnemyTypeData enemyTypeData;
    private TrailRenderer trailRenderer;

    //의존성 DIP적용 검토하기.
    public void Initialize_Enemy(InputManager _inputManager, GameServiceLocator _gameServiceLocator
        , EnemyTypeData _enemyTypeData)
    {
        base.Initialize(_inputManager, _gameServiceLocator);

        enemyTypeData = _enemyTypeData;

        SetupEnemyType();
        BindEvent();

        //trail 임시 코드.
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.material=sr.material;
        trailRenderer.material.mainTexture = sr.sprite.texture;
        Color c = trailRenderer.material.color;
        c.a = 0.3f;
        trailRenderer.material.color = c;
    }

    private void SetupEnemyType()
    {
        sr.sprite = enemyTypeData.sprite;
        float scale = enemyTypeData.scale;
        float scaleDelta = UnityEngine.Random.Range(0f, 1f);
        transform.localScale = new Vector3(scaleDelta + scale, scaleDelta + scale, 1f);
        moveComponent.SetImpulsePower(enemyTypeData.moveForce);
        healthComponent.SetHealth(enemyTypeData.health);
    }

    private void BindEvent()
    {
        gameServiceLocator.waveSystemProvider.StartMoveEvent += OnMove;
    }

    private void ReleaseEvent()
    {
        gameServiceLocator.waveSystemProvider.StartMoveEvent -= OnMove;
    }

    public override void TakeDamage(float damage)
    {
        healthComponent.DecreaseHealth(damage); 
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
        ReleaseEvent();
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