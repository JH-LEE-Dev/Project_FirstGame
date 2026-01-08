using UnityEngine;

public class GameServiceLocator
{
    private CameraController cameraController;
    private IGameFlowProvider gameFlowProvider;

    public void Initialize(CameraController _cameraController,IGameFlowProvider _gameFlowProvider)
    {
        cameraController = _cameraController;
        gameFlowProvider = _gameFlowProvider;
    }

    public void PlayCameraShake()
    {
        cameraController.Shake(2,0.5f);
    }

    public bool IsGameState<T>() where T : IState
    {
        return gameFlowProvider.IsState<T>(); 
    }

    public Camera GetMainCamera()
    {
        return cameraController.mainCam;
    }
}
