using Unity.AppUI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.EventSystems.EventTrigger;

//Component간의 결합을 줄이기 위한 Mediator 역할 + UnitContext 내부의 메서드에서 사용하는 객체들을 최대한 추상화하여
//의존성 역전도 실현
public class UnitContext
{
    public Unit unit { get; private set; }
    public Animator animator { get; private set; }
    public Vector2 moveDirection { get; private set; }

    public void Initialize(Unit unit)
    {
        this.unit = unit;
    }

    public void Update()
    {

    }

    public Unit GetUnit()
    {
        return unit;
    }

    public void SetUnitTransform(Vector2 pos)
    {
        if (unit == null)
        {
            Debug.Log("Unit is null -> UnitContext::SetUnitTransform");
            return;
        }

        unit.transform.position = pos;
    }
}
