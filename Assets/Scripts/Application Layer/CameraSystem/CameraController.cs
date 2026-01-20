using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] public Camera mainCam;
    [SerializeField] private CinemachineCamera vcam;

    CinemachineBasicMultiChannelPerlin noise;

    void Awake()
    {
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin이 vcam에 없습니다.");
        }
    }

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        noise.AmplitudeGain = 0f;
    }
}
