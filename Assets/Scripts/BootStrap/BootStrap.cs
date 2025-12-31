using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrap : MonoBehaviour, IGameFlowController
{
    private SceneController sceneManager;
    private UIInstaller uiInstaller;
    private AudioManager audioManager;
    private InputManager inputManager;

    [Header("MainMenu Level Object")]

    [Header("Gameplay Level Object")]
    [SerializeField] GameInstaller gameInstaller_Prefab;

    GameInstaller gameInstaller;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        audioManager = GetComponent<AudioManager>();
        sceneManager = GetComponent<SceneController>();
        uiInstaller = GetComponentInChildren<UIInstaller>();
        inputManager = GetComponent<InputManager>();

        uiInstaller.Initialize(this);
    }

    public void Start()
    {

    }

    public void SetupGameplayScene()
    {
        gameInstaller = Instantiate(gameInstaller_Prefab);
        gameInstaller.Initialize(inputManager);
        uiInstaller.GameplayLevelStarted();

        gameInstaller.DependencyInjection(uiInstaller);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "GameplayScene")
            SetupGameplayScene();
        else if(sceneName == "MainMenuScene")
            SetupMainMenuScene();
    }

    public void SetupMainMenuScene()
    {
        uiInstaller.MainMenuLevelStarted();
    }

    public void GoToMainMenuScene()
    {
        sceneManager.ChangeScene(SceneType.MainMenu);
    }

    public void GoToGameplayScene()
    {
        sceneManager.ChangeScene(SceneType.Gameplay);
    }
}
