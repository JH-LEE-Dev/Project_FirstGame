using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ChangeScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.MainMenu:
                SceneManager.LoadScene("MainMenuScene");
                break;
            case SceneType.Gameplay:
                SceneManager.LoadScene("GameplayScene");
                break;
        }
    }
}
