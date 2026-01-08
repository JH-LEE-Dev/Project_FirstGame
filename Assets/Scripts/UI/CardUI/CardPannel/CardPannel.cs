using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardPannel : MonoBehaviour
{
    public Action ExitPannelEvent;

    private void Awake()
    {
        ExitPannelEvent += DeActivatePannel;
    }

    private void DeActivatePannel()
    {
        gameObject.SetActive(false);
    }
}
