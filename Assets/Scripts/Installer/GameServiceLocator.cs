using UnityEngine;

public class GameServiceLocator
{
    private CameraController cameraController;
    private GameController gameController;

    public void Initialize(CameraController _cameraController,GameController _gameController)
    {
        cameraController = _cameraController;
        gameController = _gameController;
    }

    public void PlayCameraShake()
    {
        cameraController.Shake(2,0.5f);
    }

    public bool IsGameState<T>() where T : IState
    {
        return gameController.IsState<T>(); 
    }
}
