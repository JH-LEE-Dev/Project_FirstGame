using System;
using UnityEngine;

public class Character : Unit
{
    public ICombatEffectReceiver combatEffectReceiver => combatComponent;

    public event Action PlayerAttackIsFinishedEvent;

    [Header("Arrow Object")]
    private LineRenderer lineRenderer;
    [SerializeField] private float aimLength = 10f;

    private Vector2 mousePos;
    private bool bCanAction = false;
    private Vector2 fireDir;

    public void Initialize_Character(InputManager _inputManager, GameServiceLocator _gameServiceLocator)
    {
        base.Initialize(_inputManager, _gameServiceLocator);

        lineRenderer = GetComponent<LineRenderer>();

        BindEvent();
    }

    public override void TakeDamage(float damage)
    {

    }

    protected override void Update()
    {
        base.Update();

        UpdateAimLine();
    }

    public void UpdateAimLine()
    {
        if (bCanAction == false)
            return;

        Camera mainCam = gameServiceLocator.GetMainCamera();
        Vector2 origin = transform.position;

        Vector2 mouseWorldPos =
            mainCam.ScreenToWorldPoint(mousePos);

        Vector2 dir = (mouseWorldPos - origin).normalized;
        fireDir = dir;

        Vector2 endPos = origin + dir * aimLength;

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
    }

    public void SetArrowObjectTransform(Vector2 move)
    {
        mousePos = move;
    }

    protected override void OnDestroy()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetArrowObjectTransform;
        inputManager.inputReader.FireButtonPressedEvent -= Fire;
        combatComponent.BulletEffectIsFinishedEvent -= PlayeShotEffectIsFinished;
    }

    public void SetbCanAction()
    {
        lineRenderer.enabled = true;
        bCanAction = true;
    }

    public void ResetbCanAction()
    {
        lineRenderer.enabled = false;
        bCanAction = false;
    }

    private void Fire()
    {
        if (bCanAction == true)
        {
            combatComponent.Fire(fireDir);
            //Sound.Play("Fire", transform.position);
        }
    }

    private void PlayeShotEffectIsFinished()
    {
        bCanAction = false;
        PlayerAttackIsFinishedEvent?.Invoke();
    }

    private void BindEvent()
    {
        inputManager.inputReader.MoveEvent -= OnMove;
        inputManager.inputReader.MoveEvent += OnMove;
        inputManager.inputReader.PointerPositionEvent -= SetArrowObjectTransform;
        inputManager.inputReader.PointerPositionEvent += SetArrowObjectTransform;
        inputManager.inputReader.FireButtonPressedEvent -= Fire;
        inputManager.inputReader.FireButtonPressedEvent += Fire;

        combatComponent.BulletEffectIsFinishedEvent -= PlayeShotEffectIsFinished;
        combatComponent.BulletEffectIsFinishedEvent += PlayeShotEffectIsFinished;
    }
}
