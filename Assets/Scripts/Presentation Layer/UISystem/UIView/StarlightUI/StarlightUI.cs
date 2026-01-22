using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class StarlightUI : MonoBehaviour
{

    [SerializeField] private List<RectTransform> pivots;
    [SerializeField] private List<StarlightSubUI> starlightSubUIs;

    [Button]
    public void TestOn1()
    {
        ActivateSubUI(0);
    }
    [Button]
    public void TestOn2()
    {
        ActivateSubUI(1);
    }
    [Button]
    public void TestOn3()
    {
        ActivateSubUI(2);
    }
    [Button]
    public void TestOff1()
    {
        DeactivateSubUI(0);
    }
    [Button]
    public void TestOff2()
    {
        DeactivateSubUI(1);
    }
    [Button]
    public void TestOff3()
    {
        DeactivateSubUI(2);
    }

    public void Awake()
    {
        for (int i = 0; i < starlightSubUIs.Count; i++)
            starlightSubUIs[i].Init();
    }


    public void ActivateSubUI(int uiIndex)
    {
        if (uiIndex < 0 || uiIndex >= starlightSubUIs.Count) return;

        var ui = starlightSubUIs[uiIndex];

        if (!ui.GetSubUIActive())
        {
            Vector2 startPos = GetAssignedPivotPosIfActivated(uiIndex);
            ui.StartSubUIActive(startPos);
        }

        Relayout();
    }

    public void DeactivateSubUI(int uiIndex)
    {
        if (uiIndex < 0 || uiIndex >= starlightSubUIs.Count) return;

        var ui = starlightSubUIs[uiIndex];
        ui.ForceDeactivate();
        Relayout();
    }

    private void Relayout()
    {
        int pivotCursor = 0;

        for (int i = 0; i < starlightSubUIs.Count; i++)
        {
            var ui = starlightSubUIs[i];
            if (!ui.GetSubUIActive()) continue;

            if (pivotCursor >= pivots.Count) break;

            Vector2 pivotPos = pivots[pivotCursor].localPosition;
            ui.SetPosition(pivotPos);
            pivotCursor++;
        }
    }
    private Vector2 GetAssignedPivotPosIfActivated(int uiIndex)
    {
        int pivotCursor = 0;

        for (int i = 0; i < starlightSubUIs.Count; i++)
        {
            bool isActiveOrThis = starlightSubUIs[i].GetSubUIActive() || (i == uiIndex);
            if (!isActiveOrThis) continue;

            if (pivotCursor >= pivots.Count) break;

            if (i == uiIndex)
                return pivots[pivotCursor].localPosition;

            pivotCursor++;
        }

        return pivots.Count > 0 ? (Vector2)pivots[0].localPosition : Vector2.zero;
    }
}
