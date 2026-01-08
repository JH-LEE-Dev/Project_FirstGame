using Unity.AppUI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.EventSystems.EventTrigger;

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
