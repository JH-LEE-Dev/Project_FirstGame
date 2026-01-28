using UnityEngine;

public class EnvironmentManager : MonoBehaviour, IOrbitPathProvider
{
    private OrbitPathComponent orbitPathComponent;
    private FallBoundaryComponent fallBoundaryLineComponent;

    public void Initialize()
    {

    }

    public void Release()
    {

    }

    public Vector3 GetPathPosition(float value)
    {
        return orbitPathComponent.GetPathPosition(value);
    }

    public void SetPathActive(bool value)
    {
        orbitPathComponent.SetPathActive(value);
        fallBoundaryLineComponent.SetPathActive(value);
    }

    private void Awake()
    {
        orbitPathComponent = GetComponentInChildren<OrbitPathComponent>();
        fallBoundaryLineComponent = GetComponentInChildren<FallBoundaryComponent>();
    }
}
