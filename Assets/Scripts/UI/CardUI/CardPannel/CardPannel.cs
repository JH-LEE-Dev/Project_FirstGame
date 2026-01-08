using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPannel : MonoBehaviour
{
    public Action ExitPannelEvent;

    private ScrollRect pannelScroll = null;

    private void Awake()
    {
        pannelScroll = gameObject.GetComponentInChildren<ScrollRect>();

        ExitPannelEvent += DeActivatePannel;
    }

    private void DeActivatePannel()
    {
        gameObject.SetActive(false);
        pannelScroll.verticalNormalizedPosition = 1f;
    }
}
