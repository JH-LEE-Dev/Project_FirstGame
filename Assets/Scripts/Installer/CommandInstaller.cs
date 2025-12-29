using UnityEngine;

public class CommandInstaller
{
    private InputManager inputManager;
    private CommandManager commandManager;
    private CommandFactory commandFactory;
    private CommandDispatcher commandDispatcher;

    public void Initialize(InputManager _inputManager)
    {
        inputManager = _inputManager;
        commandManager = new CommandManager();
        commandDispatcher = new CommandDispatcher();
        commandFactory = new CommandFactory();

        if (commandManager == null || commandDispatcher == null || commandFactory == null || inputManager == null)
        {
            Debug.Log("Something is null -> CommandInstaller::Initialize");
            return;
        }

    }

    public void Release()
    {

    }
}
