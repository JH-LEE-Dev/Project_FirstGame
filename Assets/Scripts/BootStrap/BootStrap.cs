using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootStrap : MonoBehaviour, IGameFlowController
{
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
        SetupMainMenuScene();
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

        SceneManager.sceneLoaded += OnSceneLoaded;

        audioManager = GetComponent<AudioManager>();
        sceneManager = GetComponent<SceneController>();
        uiInstaller = GetComponentInChildren<UIInstaller>();
        inputManager = GetComponent<InputManager>();

        inputManager.Initialize();
        uiInstaller.Initialize(this,inputManager);

        inputManager.inputReader.ESCButtonPressedEvent -= GoToMainMenuScene;
        inputManager.inputReader.ESCButtonPressedEvent += GoToMainMenuScene;
    }

    public void Start()
    {
        //Sound.PlayBGM("BGM_MainMenu");
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetupGameplayScene()
    {
        gameInstaller = Instantiate(gameInstaller_Prefab);
        gameInstaller.Initialize(inputManager);
        gameInstaller.DependencyInjection_Gameplay(uiInstaller);

        uiInstaller.GameplayLevelStarted();
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
