using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour, IOrbitPathProvider
{
    //외부 의존성
    IUnitSpawnSystemData unitSpawnSystemData;

    //내부 의존성
    private OrbitPathComponent orbitPathComponent;
    private FallBoundaryComponent fallBoundaryLineComponent;

    public void Initialize(IUnitSpawnSystemData _unitSpawnSystemData)
    {
        unitSpawnSystemData = _unitSpawnSystemData;

        orbitPathComponent = GetComponentInChildren<OrbitPathComponent>();
        fallBoundaryLineComponent = GetComponentInChildren<FallBoundaryComponent>();

        orbitPathComponent.Initialize();
        fallBoundaryLineComponent.Initialize(unitSpawnSystemData.enemiesData);
    }

    public void Release()
    {

    }

    // OrbitPath
    public Vector3 GetPathPosition(float value)
    {
        return orbitPathComponent.GetPathPosition(value);
    }

    public void SetPathActive(bool value)
    {
        orbitPathComponent.SetPathActive(value);
        fallBoundaryLineComponent.SetPathActive(value);
    }

    // FallBoundaryComponent
}
