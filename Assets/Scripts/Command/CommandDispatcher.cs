using System.Windows.Input;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CommandDispatcher
{
    public void Dispatch(Unit target, ICommand command)
    {
        if (target == null || command == null)
        {
            Debug.Log("Something is null -> CommandDispatcher::Dispatch");
            return;
        }

        target.EnqueueCommand(command);
    }
}

