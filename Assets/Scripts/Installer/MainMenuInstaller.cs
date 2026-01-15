using UnityEngine;

public class MainMenuInstaller : MonoBehaviour
{
    //외부 의존성
    private InputManager inputManager;
    private MainMenuUIInstaller uiInstaller;
    private IBootStrapProvider bootStrapProvider;

    public void Initialize(IBootStrapProvider _bootStrapProvider, InputManager _inputManager)
    {
        inputManager = _inputManager;
        bootStrapProvider = _bootStrapProvider;

        uiInstaller = GetComponentInChildren<MainMenuUIInstaller>();
        uiInstaller.Initialize(bootStrapProvider, inputManager);
    }

    public void Release()
    {
        uiInstaller.Release();
    }

    public void StartMainMenuScene()
    {
        uiInstaller.MainMenuLevelStarted();
    }
}
