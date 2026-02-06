using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class StarlightUI : MonoBehaviour
{

    [SerializeField] private List<RectTransform> pivots;
    [SerializeField] private List<StarlightSubUI> starlightSubUIs;

    [SerializeField] private TextMeshProUGUI starlightTM;
    [SerializeField] private TextMeshProUGUI starlightAddCountTM;

    // --- Wave anim tuning ---
    [SerializeField] private float waveFoldGap = 0.20f;

    [Header("Total Add Anim (Hit)")]
    [SerializeField] private float totalAddHitDur = 0.20f;
    [SerializeField] private float totalAddPunchScale = 0.25f;
    [SerializeField] private int totalAddPunchVibrato = 8;
    [SerializeField, Range(0f, 1f)] private float totalAddPunchElasticity = 0.8f;
    [SerializeField] private float totalAddShakeStrengthMin = 6f;
    [SerializeField] private float totalAddShakeStrengthMax = 10f;
    [SerializeField] private int totalAddShakeVibrato = 20;
    [SerializeField] private float totalAddShakeRandomness = 90f;
    [SerializeField] private int totalSeedBase = 12345;

    [Header("Total Number Tween")]
    [SerializeField] private float totalNumberDur = 0.40f;
    [SerializeField] private Ease totalNumberEase = Ease.OutCubic;

    [Header("Merge Anim")]
    [SerializeField] private float mergeRightOffset = 10f;
    [SerializeField] private float mergeRightDur = 0.08f;
    [SerializeField] private float mergeSquashDur = 0.06f;
    [SerializeField] private float mergeMoveDur = 0.18f;
    [SerializeField] private float mergeWaitAfterLastAdd = 0.50f;
    [SerializeField] private Vector2 mergeSquashScale = new Vector2(0.75f, 1.15f);
    [SerializeField] private Vector2 mergeStretchScale = new Vector2(1.15f, 0.80f);

    private readonly Color addUpGreen = new Color32(71, 226, 93, 255);

    // --- internal ---
    private RectTransform starTMRT;
    private RectTransform starAddRT;
    private Vector2 starAddBasePos;
    private Vector3 starAddBaseScale;

    private int starBase = 0;
    public int GetStarlight() => starBase;
    private int starAddTotal = 0;
    private int starBaseDisplayed = 0;
    private int starAddDisplayed = 0;

    private Tween starAddNumberTween;
    private Tween starAddColorTween;
    private Tween starAddPunchTween;
    private Tween starAddShakeTween;

    private Tween starBaseNumberTween;
    private Tween starBaseColorTween;
    private Tween starBasePunchTween;

    private Sequence mergeSeq;

    private int hitCounter = 0;
    private Coroutine waveRoutine;

    private void Awake()
    {
        for (int i = 0; i < starlightSubUIs.Count; i++)
            starlightSubUIs[i].Init();

        if (starlightTM) starTMRT = starlightTM.rectTransform;
        if (starlightAddCountTM)
        {
            starAddRT = starlightAddCountTM.rectTransform;
            starAddBasePos = starAddRT.anchoredPosition;
            starAddBaseScale = starAddRT.localScale;

            starlightAddCountTM.gameObject.SetActive(false);
            var c = starlightAddCountTM.color; c.a = 0f; starlightAddCountTM.color = c;
        }

        starBaseDisplayed = starBase;
        starAddDisplayed = 0;
        if (starlightTM) starlightTM.text = starBaseDisplayed.ToString();
    }

    public void ActivateSubUI(StarLightAcquisitionType type, int addValue)
    {
        int idx = IndexHeshing(type);

        if (idx < 0 || idx >= starlightSubUIs.Count) return;

        var ui = starlightSubUIs[idx];

        ui.AddToAddCount(addValue);

        if (!ui.GetSubUIActive())
        {
            Vector2 startPos = GetAssignedPivotPosIfActivated(idx);
            ui.StartSubUIActive(startPos);
        }

        Relayout();
    }

    public void TurnAdjustment()
    {
        StartCoroutine(AllAdjustmentRoutine());
    }

    public void WaveAdjustment()
    {
        if (waveRoutine != null) StopCoroutine(waveRoutine);
        waveRoutine = StartCoroutine(WaveAdjustmentRoutine());
    }



    /// <summary>
    /// 아래는 건들지 마시오.
    /// </summary>

    private IEnumerator AllAdjustmentRoutine()
    {
        const float gap = 0.1f;

        for (int i = 0; i < starlightSubUIs.Count; i++)
        {
            var ui = starlightSubUIs[i];
            if (!ui.GetSubUIActive()) continue;

            ui.Adjustment();
            yield return new WaitForSeconds(gap);
        }
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


    // To Adjustment
    private void AddToStarlightAdd(int delta)
    {
        if (delta <= 0 || !starlightAddCountTM || starAddRT == null) return;

        int prev = starAddTotal;
        starAddTotal += delta;

        if (starAddTotal <= 0)
        {
            HideStarAdd();
            return;
        }

        starlightAddCountTM.gameObject.SetActive(true);
        var col = starlightAddCountTM.color; col.a = 1f; starlightAddCountTM.color = col;

        TweenStarAddNumberTo(starAddTotal);

        PlayStarAddHit();
        PlayStarAddColorPulse();
    }

    private void TweenStarAddNumberTo(int target)
    {
        starAddNumberTween?.Kill();
        int end = Mathf.Max(0, target);

        starAddNumberTween = DOTween.To(
            () => starAddDisplayed,
            v =>
            {
                starAddDisplayed = v;
                if (starlightAddCountTM && starAddTotal > 0)
                    starlightAddCountTM.text = $"+{starAddDisplayed}";
            },
            end,
            totalNumberDur
        ).SetEase(totalNumberEase);
    }

    private void PlayStarAddColorPulse()
    {
        starAddColorTween?.Kill();

        starlightAddCountTM.color = addUpGreen;
        starAddColorTween = starlightAddCountTM.DOColor(Color.white, totalNumberDur)
                                             .SetEase(Ease.OutCubic);
    }

    private void PlayStarAddHit()
    {
        starAddPunchTween?.Kill();
        starAddShakeTween?.Kill();

        // 기준 복구(탈출 방지)
        starAddRT.localScale = starAddBaseScale;
        starAddRT.anchoredPosition = starAddBasePos;

        int seed = totalSeedBase + (++hitCounter);
        var rng = new System.Random(seed);
        float strength = Mathf.Lerp(totalAddShakeStrengthMin, totalAddShakeStrengthMax, (float)rng.NextDouble());

        starAddPunchTween = starAddRT.DOPunchScale(
            Vector3.one * totalAddPunchScale,
            totalAddHitDur,
            totalAddPunchVibrato,
            totalAddPunchElasticity
        );

        starAddShakeTween = starAddRT.DOShakeAnchorPos(
            totalAddHitDur,
            new Vector2(strength, strength * 0.7f),
            totalAddShakeVibrato,
            totalAddShakeRandomness,
            snapping: false,
            fadeOut: true
        ).OnComplete(() => starAddRT.anchoredPosition = starAddBasePos);
    }

    private void HideStarAdd()
    {
        starAddTotal = 0;
        starAddDisplayed = 0;

        starAddNumberTween?.Kill();
        starAddColorTween?.Kill();
        starAddPunchTween?.Kill();
        starAddShakeTween?.Kill();

        if (!starlightAddCountTM) return;

        starlightAddCountTM.gameObject.SetActive(false);
        starlightAddCountTM.text = "";
        var c = starlightAddCountTM.color; c.a = 0f; starlightAddCountTM.color = c;

        if (starAddRT)
        {
            starAddRT.anchoredPosition = starAddBasePos;
            starAddRT.localScale = starAddBaseScale;
        }
    }

    private IEnumerator MergeStarAddIntoBase()
    {
        if (starAddTotal <= 0 || !starlightAddCountTM || !starlightTM || starAddRT == null || starTMRT == null)
            yield break;

        starAddNumberTween?.Kill();
        starAddColorTween?.Kill();
        starAddPunchTween?.Kill();
        starAddShakeTween?.Kill();

        mergeSeq?.Kill();
        mergeSeq = DOTween.Sequence();

        starlightAddCountTM.gameObject.SetActive(true);
        var col = starlightAddCountTM.color; col.a = 1f; starlightAddCountTM.color = col;

        starAddRT.anchoredPosition = starAddBasePos;
        starAddRT.localScale = starAddBaseScale;

        Vector2 countPos = starTMRT.anchoredPosition;
        Vector2 rightPos = starAddBasePos + Vector2.right * mergeRightOffset;

        mergeSeq.Append(starAddRT.DOAnchorPos(rightPos, mergeRightDur).SetEase(Ease.OutCubic));
        mergeSeq.Join(starAddRT.DOScale(new Vector3(mergeSquashScale.x, mergeSquashScale.y, 1f), mergeSquashDur).SetEase(Ease.OutCubic));
        mergeSeq.Append(starAddRT.DOScale(new Vector3(mergeStretchScale.x, mergeStretchScale.y, 1f), 0.10f).SetEase(Ease.OutCubic));
        mergeSeq.Join(starAddRT.DOAnchorPos(countPos, mergeMoveDur).SetEase(Ease.InCubic));
        mergeSeq.Join(starlightAddCountTM.DOFade(0f, mergeMoveDur).SetEase(Ease.InCubic));
        mergeSeq.Append(starAddRT.DOScale(starAddBaseScale, 0.08f).SetEase(Ease.OutCubic));

        int delta = starAddTotal;
        int endBase = starBase + delta;
        starBase = endBase;

        TweenStarBaseNumberTo(endBase);
        PlayStarBasePulseAndHit();

        yield return mergeSeq.WaitForCompletion();

        HideStarAdd();
    }

    private void TweenStarBaseNumberTo(int target)
    {
        starBaseNumberTween?.Kill();
        int end = Mathf.Max(0, target);

        starBaseNumberTween = DOTween.To(
            () => starBaseDisplayed,
            v =>
            {
                starBaseDisplayed = v;
                if (starlightTM) starlightTM.text = starBaseDisplayed.ToString();
            },
            end,
            totalNumberDur
        ).SetEase(totalNumberEase);
    }

    private void PlayStarBasePulseAndHit()
    {
        if (!starlightTM || starTMRT == null) return;

        starBaseColorTween?.Kill();
        starBasePunchTween?.Kill();

        starlightTM.color = addUpGreen;
        starBaseColorTween = starlightTM.DOColor(Color.white, totalNumberDur).SetEase(Ease.OutCubic);

        starTMRT.localScale = Vector3.one;
        starBasePunchTween = starTMRT.DOPunchScale(Vector3.one * 0.12f, 0.18f, 10, 0.8f);
    }

    private IEnumerator WaveAdjustmentRoutine()
    {
        List<StarlightSubUI> ordered = GetActiveUIsInPivotOrder();
        if (ordered.Count == 0) yield break;

        bool lastArrived = false;

        for (int i = 0; i < ordered.Count; i++)
        {
            var ui = ordered[i];
            int baseMoney = ui.GetBaseCount();

            bool isLast = (i == ordered.Count - 1);

            ui.WaveFoldToY0(
                moveDur: 0.20f,
                fadeDur: 0.20f,
                ease: Ease.InCubic,
                onArrive: () =>
                {
                    AddToStarlightAdd(baseMoney);
                    ui.ResetAllCounts();
                    if (isLast) lastArrived = true;
                });

            yield return new WaitForSeconds(waveFoldGap);
        }

        float safety = 2.0f;
        while (!lastArrived && safety > 0f)
        {
            safety -= Time.deltaTime;
            yield return null;
        }

        if (starAddNumberTween != null && starAddNumberTween.active)
            yield return starAddNumberTween.WaitForCompletion();

        yield return new WaitForSeconds(mergeWaitAfterLastAdd);

        yield return MergeStarAddIntoBase();

        waveRoutine = null;
    }

    private List<StarlightSubUI> GetActiveUIsInPivotOrder()
    {
        List<StarlightSubUI> list = new();
        int pivotCursor = 0;

        for (int i = 0; i < starlightSubUIs.Count; i++)
        {
            var ui = starlightSubUIs[i];
            if (!ui.GetSubUIActive()) continue;
            if (pivotCursor >= pivots.Count) break;

            list.Add(ui);
            pivotCursor++;
        }
        return list;
    }

    private int IndexHeshing(StarLightAcquisitionType type)
    {
        if (type == StarLightAcquisitionType.Kill)
            return 0;
        else if (type == StarLightAcquisitionType.Ability)
            return 1;
        else
            return 2;
    }
}
