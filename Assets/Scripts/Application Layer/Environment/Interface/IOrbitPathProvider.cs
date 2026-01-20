using UnityEngine;

public interface IOrbitPathProvider
{
    Vector3 GetPathPosition(float value);

    public void SetPathActive(bool value);
}
