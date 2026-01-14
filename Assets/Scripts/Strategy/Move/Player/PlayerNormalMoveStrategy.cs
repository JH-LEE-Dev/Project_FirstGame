using UnityEngine;


[CreateAssetMenu(menuName = "Strategy/Move/PlayerNormalMove")]
public class PlayerNormalMoveStrategy : PlayerMoveStrategy
{
    public override void Initialize(Unit _unit, IOrbitPathProvider _orbitPathProvider, IMoveSignalHandler _moveSignalHandler)
    {
        unit = _unit;
        orbitPathProvider = _orbitPathProvider;
        moveSignalHandler = _moveSignalHandler;
        ApplyPosition();
    }


    public override void Move(Vector2 direction)
    {
        if (unit == null || orbitPathProvider == null)
            return;

        if (direction.x == -1f)
        {
            moveSignalHandler.NotifyMoveSignalAction(MoveActionSignal.LeftMoving);
        }
        else if (direction.x == 1f)
        {
            moveSignalHandler.NotifyMoveSignalAction(MoveActionSignal.RightMoving);
        }
        else
        {
            moveSignalHandler.NotifyMoveSignalAction(MoveActionSignal.Idle);
        }


        float dt = Time.deltaTime;

        inputAxis = -Mathf.Clamp(direction.x, -1f, 1f);

        targetSpeed = inputAxis * maxSpeedPerSec;

        if (Mathf.Approximately(movingValue, 0f) && targetSpeed < 0f)
            targetSpeed = 0f;
        if (Mathf.Approximately(movingValue, 1f) && targetSpeed > 0f)
            targetSpeed = 0f;


        bool pushingRightWall = Mathf.Approximately(movingValue, 0f) && inputAxis < 0f;
        bool pushingLeftWall = Mathf.Approximately(movingValue, 1f) && inputAxis > 0f;

        if (pushingLeftWall)
            moveSignalHandler.NotifyMoveSignalAction(MoveActionSignal.LeftBlocked);
        else if (pushingRightWall)
            moveSignalHandler.NotifyMoveSignalAction(MoveActionSignal.RightBlocked);
        else
            moveSignalHandler.NotifyMoveSignalAction(MoveActionSignal.NotBlocked);



        bool stopping = Mathf.Approximately(targetSpeed, 0f);
        float timeConstant = stopping ? decelTime : accelTime;

        float lerpFactor = 1f - Mathf.Exp(-dt / Mathf.Max(timeConstant, Mathf.Epsilon));
        float curved = stopping ? decelCurve.Evaluate(lerpFactor) : accelCurve.Evaluate(lerpFactor);

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, curved);

        movingValue += currentSpeed * dt;

        float clamped = Mathf.Clamp01(movingValue);
        if (!Mathf.Approximately(clamped, movingValue))
        {
            movingValue = clamped;
            currentSpeed = 0f;
        }
        else
        {
            movingValue = clamped;
        }

        ApplyPosition();
    }

}
