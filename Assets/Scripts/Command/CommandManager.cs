using System.Drawing;
using System.Windows.Input;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CommandManager
{
    [SerializeField] private CommandFactory factory;
    [SerializeField] private CommandDispatcher dispatcher;

    public void Initialize(CommandFactory factory, CommandDispatcher dispatcher)
    {
        this.factory = factory;
        this.dispatcher = dispatcher;
    }
}
