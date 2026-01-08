using System;
using UnityEngine;

[Serializable]
public struct CanvasRoot
{
   public Transform screenLayerRoot;
   public Transform popupLayerRoot;
   public Transform overlayLayerRoot;
   public Transform tooltipLayerRoot;
}
