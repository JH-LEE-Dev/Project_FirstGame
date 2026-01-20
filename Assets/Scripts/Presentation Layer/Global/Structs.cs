using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct CanvasRoot
{
    public Transform screenLayerRoot;
    public Transform popupLayerRoot;
    public Transform overlayLayerRoot;
    public Transform tooltipLayerRoot;
    public Transform worldLayerRoot;
}