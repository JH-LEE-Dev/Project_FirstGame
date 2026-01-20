using UnityEngine;

public class GameServiceLocator
{
    private CameraController cameraController;

    public void Initialize(CameraController _cameraController)
    {
        cameraController = _cameraController;
    }

    public void PlayCameraShake()
    {
        cameraController.Shake(2,0.5f);
    }

    public Camera GetMainCamera()
    {
        return cameraController.mainCam;
    }
}
