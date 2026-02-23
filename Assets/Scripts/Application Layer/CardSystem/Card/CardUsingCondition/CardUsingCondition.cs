using UnityEngine;
using System;

public abstract class CardUsingCondition : ScriptableObject
{
    protected CardDataInstance ownerCard;

    public bool bResult = false;
    public abstract void CheckUsingCondition(IComplexSystemActionCommandHandler complexSystemActionCommandHandler);
}
