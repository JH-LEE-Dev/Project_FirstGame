using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public interface ICommand
{
    void Execute(Unit unit);
}
