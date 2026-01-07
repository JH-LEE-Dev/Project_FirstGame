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

namespace SystemAction
{
    public struct DrawAgain
    {
        public int drawAmount;
    }
}
