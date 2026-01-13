using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BootStrap : MonoBehaviour, IBootStrapProvider
{
    [SerializeField] bool bTempScene = false;

    private static BootStrap Instance;

    private SceneController sceneManager;
    private UIInstaller uiInstaller;
    private AudioManager audioManager;
    private InputManager inputManager;

    [Header("MainMenu Level Object")]

    [Header("Gameplay Level Object")]
    [SerializeField] GameInstaller gameInstaller_Prefab;

    private GameInstaller gameInstaller;

    private void BootTempScene()
    {
        SetupGameplayScene();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        audioManager = GetComponent<AudioManager>();
        sceneManager = GetComponent<SceneController>();
        uiInstaller = GetComponentInChildren<UIInstaller>();
        inputManager = GetComponent<InputManager>();

        inputManager.Initialize();
        uiInstaller.Initialize(this, inputManager);

        BindEvent();
    }

    public void Start()
    {
        if (bTempScene)
            BootTempScene();

        //Sound.PlayBGM("BGM_MainMenu");
    }

    public void OnDestroy()
    {
        ReleaseEvent();
    }

    private void BindEvent()
    {
        inputManager.inputReader.ESCButtonPressedEvent -= GoToMainMenuScene;
        inputManager.inputReader.ESCButtonPressedEvent += GoToMainMenuScene;
    }

    private void ReleaseEvent()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (inputManager != null)
            inputManager.inputReader.ESCButtonPressedEvent -= GoToMainMenuScene;
    }

    public void SetupGameplayScene()
    {
        gameInstaller = Instantiate(gameInstaller_Prefab);
        uiInstaller.DependencyInjection_Gameplay(gameInstaller);
        gameInstaller.Initialize(inputManager);
        gameInstaller.DependencyInjection_Gameplay(uiInstaller);

        uiInstaller.GameplayLevelStarted();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "GameplayScene")
            SetupGameplayScene();
        else if (sceneName == "MainMenuScene")
            SetupMainMenuScene();
    }

    public void SetupMainMenuScene()
    {
        uiInstaller.MainMenuLevelStarted();
    }

    public void GoToMainMenuScene()
    {
        if (bTempScene)
            return;

        uiInstaller.Release_Gameplay();
        sceneManager.ChangeScene(SceneType.MainMenu);
    }

    public void GoToGameplayScene()
    {
        uiInstaller.Release_MainMenu();
        sceneManager.ChangeScene(SceneType.Gameplay);
    }
}
