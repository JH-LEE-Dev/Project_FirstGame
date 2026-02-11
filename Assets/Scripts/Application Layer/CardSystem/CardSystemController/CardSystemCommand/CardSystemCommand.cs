using UnityEngine;

public abstract class CardSystemCommand : GameSystemCommand
{
    public bool IsActive { get; private set; }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;


}
