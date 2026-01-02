using UnityEngine;

public class Character : Unit
{
    [Header("Arrow Object")]
    [SerializeField] private GameObject arrowObject;

    public override void Initialize(InputManager _inputManager,GameServiceLocator _gameServiceLocator, WaveManager _waveManager = null, 
        EnemyTypeData _enemyTypeData = null)
    {
        base.Initialize(_inputManager, _gameServiceLocator, _waveManager);

        inputManager.inputReader.MoveEvent += OnMove;
        inputManager.inputReader.PointerPositionEvent += SetArrowObjectTransform;
    }

    public override void TakeDamage(float damage)
    {

    }

    protected override void Update()
    {
        base.Update();


    }

    public void SetArrowObjectTransform(Vector2 move)
    {

    }

    protected override void OnDestroy()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetArrowObjectTransform;
    }
}
