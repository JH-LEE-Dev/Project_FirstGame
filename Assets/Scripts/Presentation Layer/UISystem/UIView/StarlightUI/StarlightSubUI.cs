using DG.Tweening;
using TMPro;
using UnityEngine;


public class StarlightSubUI : MonoBehaviour
{
    private bool bActive = false;
    private Vector2 targetPos = Vector2.zero;

    [SerializeField] private CanvasGroup abilityGroup;


    [SerializeField] private TextMeshProUGUI countTM;
    [SerializeField] private TextMeshProUGUI addCountTM;
    [SerializeField] private RectTransform addCountPivot;

    [Header("Tween")]
    [SerializeField] private float appearOffset = 10f;
    [SerializeField] private float appearDur = 0.18f;
    [SerializeField] private float moveDur = 0.20f;
    [SerializeField] private Ease appearEase = Ease.OutCubic;
    [SerializeField] private Ease moveEase = Ease.OutCubic;
    private Tween moveTween;
    private Tween fadeTween;

    [Header("AddCount Anim")]
    [SerializeField] private float addAppearDur = 0.20f;
    [SerializeField] private float addHitDur = 0.20f;
    [SerializeField] private float addPunchScale = 0.25f;
    [SerializeField] private int addPunchVibrato = 8;
    [SerializeField, Range(0f, 1f)] private float addPunchElasticity = 0.8f;

    [SerializeField] private float addShakeStrengthMin = 6f;
    [SerializeField] private float addShakeStrengthMax = 10f;
    [SerializeField] private int addShakeVibrato = 20;
    [SerializeField] private float addShakeRandomness = 90f;
    [SerializeField] private int addSeedBase = 12345;


    [SerializeField] private float addNumberDur = 0.40f;
    [SerializeField] private Ease addNumberEase = Ease.OutCubic;

    private readonly Color addUpRed = new Color32(184, 227, 70, 255);
    private readonly Color addGreenBase = new Color32(71, 226, 93, 255);

    private Tween addNumberTween;
    private Tween addColorTween;
    private int addDisplayed = 0;

    private Tween addMoveTween;
    private Tween addFadeTween;
    private Tween addPunchTween;
    private Tween addShakeTween;

    private int baseCount = 0;
    public int GetBaseCount() => baseCount;
    private int addTotal = 0;

    // AddCountPivot 기준값 캐시
    private RectTransform addTMRT;
    private Vector2 addTMBasePos;
    private Vector3 addTMBaseScale;


    private int addHitCounter = 0;


    // CountTM 메인애니메이션
    private RectTransform countTMRT;
    private Vector3 countBaseScale;
    private Tween countNumberTween;
    private Tween countColorTween;
    private Tween countShakeTween;
    private int baseDisplayed = 0;

    private Sequence adjustSeq;
    private bool isAdjusting = false;

    [SerializeField] private float adjustIntervalRightDur = 0.08f;
    [SerializeField] private float adjustMoveToCountDur = 0.18f;
    [SerializeField] private float adjustSquashDur = 0.06f;
    [SerializeField] private float adjustStretchDur = 0.10f;

    [SerializeField] private float adjustRightOffset = 10f;

    // 쫀쫀 스케일
    [SerializeField] private Vector2 squashScale = new Vector2(0.75f, 1.15f); 
    [SerializeField] private Vector2 stretchScale = new Vector2(1.15f, 0.80f);

    // count 증가 연출
    [SerializeField] private float countNumberDur = 0.40f;
    [SerializeField] private Ease countNumberEase = Ease.OutCubic;

    public void Init()
    {
        bActive = false;

        baseCount = 0;
        baseDisplayed = baseCount;

        addTotal = 0;
        addDisplayed = 0;
        addHitCounter = 0;

        // countTM 캐시
        if (countTM)
        {
            countTMRT = countTM.rectTransform;
            countBaseScale = countTMRT.localScale;
            countTM.color = Color.white;
        }

        // addTM 캐시
        if (addCountTM)
        {
            addTMRT = addCountTM.rectTransform;
            addTMBasePos = addTMRT.anchoredPosition;
            addTMBaseScale = addTMRT.localScale;

            var c = addCountTM.color; c.a = 0f; addCountTM.color = c;
            addCountTM.gameObject.SetActive(false);
        }

        RenderCounts();

        abilityGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void AddToAddCount(int addValue)
    {
        int prevTotal = addTotal;
        addTotal += addValue;

        if (!addCountTM || addTMRT == null) return;

        if (addTotal <= 0)
        {
            KillAddTweens();
            addNumberTween?.Kill();
            addColorTween?.Kill();

            addDisplayed = 0;
            addCountTM.gameObject.SetActive(false);

            var c = addCountTM.color; c.a = 0f; addCountTM.color = c;
            addTMRT.anchoredPosition = addTMBasePos;
            addTMRT.localScale = addTMBaseScale;
            return;
        }

        addCountTM.gameObject.SetActive(true);

        if (prevTotal <= 0)
        {
            addDisplayed = 0;
            addCountTM.text = $"+{addDisplayed}";

            PlayAddAppear();
            TweenAddNumberTo(addTotal);       
            PlayAddGreenPulse();              
        }
        else
        {
            if (addValue > 0)
            {
                PlayAddHit();
                TweenAddNumberTo(addTotal);
                PlayAddGreenPulse();
            }
            else
            {
                TweenAddNumberTo(addTotal);
            }
        }
    }

    private void RenderCounts()
    {
        if (countTM) countTM.text = baseDisplayed.ToString();

        if (addCountTM)
        {
            if (addTotal > 0)
                addCountTM.text = $"+{addDisplayed}";
            else
                addCountTM.text = "";
        }
    }

    // 숫자 증가 연출 색상
    private void PlayAddGreenPulse()
    {
        if (!addCountTM) return;

        addColorTween?.Kill();

        addCountTM.color = addUpRed;
        addColorTween = addCountTM.DOColor(addGreenBase, addNumberDur)
                                  .SetEase(Ease.OutCubic);
    }

    // 숫자 증가 연출
    private void TweenAddNumberTo(int target)
    {
        addNumberTween?.Kill();

        // 시작값은 현재 표시값
        int start = addDisplayed;
        int end = Mathf.Max(0, target);

        // TMP 텍스트 갱신
        addNumberTween = DOTween.To(
                () => addDisplayed,
                v =>
                {
                    addDisplayed = v;
                    if (addCountTM && addTotal > 0)
                        addCountTM.text = $"+{addDisplayed}";
                },
                end,
                addNumberDur
            )
            .SetEase(addNumberEase);
    }

    // 첫 등장 모션
    private void PlayAddAppear()
    {
        if (!addCountTM || addTMRT == null) return;

        KillAddTweens();

        addTMRT.localScale = addTMBaseScale;

        addTMRT.anchoredPosition = addTMBasePos + Vector2.left * appearOffset;

        var c = addCountTM.color; c.a = 0f; addCountTM.color = c;

        addMoveTween = addTMRT.DOAnchorPos(addTMBasePos, addAppearDur).SetEase(Ease.OutCubic);
        addFadeTween = addCountTM.DOFade(1f, addAppearDur).SetEase(Ease.OutCubic);
    }

    // 추가 모션
    private void PlayAddHit()
    {
        if (addTMRT == null) return;

        addPunchTween?.Kill();
        addShakeTween?.Kill();

        addTMRT.localScale = addTMBaseScale;
        addTMRT.anchoredPosition = addTMBasePos;

        int seed = addSeedBase + (++addHitCounter);
        var rng = new System.Random(seed);
        float strength = Mathf.Lerp(addShakeStrengthMin, addShakeStrengthMax, (float)rng.NextDouble());

        addPunchTween = addTMRT.DOPunchScale(
            Vector3.one * addPunchScale,
            addHitDur,
            addPunchVibrato,
            addPunchElasticity
        );

        addShakeTween = addTMRT.DOShakeAnchorPos(
            addHitDur,
            new Vector2(strength, strength * 0.7f),
            addShakeVibrato,
            addShakeRandomness,
            snapping: false,
            fadeOut: true
        )
        .OnComplete(() =>
        {
            addTMRT.anchoredPosition = addTMBasePos;
        });
    }

    private void KillAddTweens()
    {
        addMoveTween?.Kill();
        addFadeTween?.Kill();
        addPunchTween?.Kill();
        addShakeTween?.Kill();

        addNumberTween?.Kill();
        addColorTween?.Kill();
    }

    public void StartSubUIActive(Vector2 arcPos)
    {
        bActive = true;

        RectTransform rt = GetComponent<RectTransform>();
        targetPos = arcPos;

        Vector2 startPos = arcPos + Vector2.left * appearOffset;
        rt.anchoredPosition = startPos;

        moveTween?.Kill();
        fadeTween?.Kill();

        RenderCounts();

        abilityGroup.alpha = 0f;
        gameObject.SetActive(true);

        moveTween = rt.DOAnchorPos(targetPos, appearDur).SetEase(appearEase);
        fadeTween = abilityGroup.DOFade(1f, appearDur).SetEase(appearEase);
    }

    public bool GetSubUIActive() => bActive;

    public void SetPosition(Vector2 arcPos)
    {
        targetPos = arcPos;
        if (!bActive) return;

        RectTransform rt = GetComponent<RectTransform>();

        if (((Vector2)rt.anchoredPosition - targetPos).sqrMagnitude < 0.0001f)
            return;

        moveTween?.Kill();
        moveTween = rt.DOAnchorPos(targetPos, moveDur).SetEase(moveEase);
    }


    // adjustment

    public void Adjustment(System.Action onComplete = null)
    {
        if (!bActive) { onComplete?.Invoke(); return; }
        if (isAdjusting) return; // 중복 방지
        if (addTotal <= 0) { onComplete?.Invoke(); return; }

        isAdjusting = true;

        KillAddTweens();

        countNumberTween?.Kill();
        countColorTween?.Kill();
        countShakeTween?.Kill();

        addCountTM.gameObject.SetActive(true);

        addTMRT.anchoredPosition = addTMBasePos;
        addTMRT.localScale = addTMBaseScale;

        var col = addCountTM.color;
        col.a = 1f;
        addCountTM.color = col;

        Vector2 countPos = countTMRT ? countTMRT.anchoredPosition : addTMBasePos;

        Vector2 rightPos = addTMBasePos + Vector2.right * adjustRightOffset;

        adjustSeq?.Kill();
        adjustSeq = DOTween.Sequence();

        adjustSeq.Append(addTMRT.DOAnchorPos(rightPos, adjustIntervalRightDur).SetEase(Ease.OutCubic));

        adjustSeq.Join(addTMRT.DOScale(new Vector3(squashScale.x, squashScale.y, 1f), adjustSquashDur)
            .SetEase(Ease.OutCubic));

        adjustSeq.Append(addTMRT.DOScale(new Vector3(stretchScale.x, stretchScale.y, 1f), adjustStretchDur)
            .SetEase(Ease.OutCubic));

        adjustSeq.Join(addTMRT.DOAnchorPos(countPos, adjustMoveToCountDur).SetEase(Ease.InCubic));
        adjustSeq.Join(addCountTM.DOFade(0f, adjustMoveToCountDur).SetEase(Ease.InCubic));

        adjustSeq.Append(addTMRT.DOScale(addTMBaseScale, 0.08f).SetEase(Ease.OutCubic));

        int delta = addTotal;
        int startBase = baseCount;
        int endBase = baseCount + delta;

        baseCount = endBase;

        adjustSeq.OnComplete(() =>
        {
            addTotal = 0;
            addDisplayed = 0;

            addCountTM.gameObject.SetActive(false);

            addTMRT.anchoredPosition = addTMBasePos;
            addTMRT.localScale = addTMBaseScale;

            var c2 = addCountTM.color; c2.a = 0f; addCountTM.color = c2;

            isAdjusting = false;

            TweenBaseNumberTo(endBase);
            PlayCountPulseAndShake();

            onComplete?.Invoke();
        });
    }

    private void TweenBaseNumberTo(int target)
    {
        countNumberTween?.Kill();
        int end = Mathf.Max(0, target);

        countNumberTween = DOTween.To(
            () => baseDisplayed,
            v =>
            {
                baseDisplayed = v;
                if (countTM) countTM.text = baseDisplayed.ToString();
            },
            end,
            countNumberDur
        ).SetEase(countNumberEase);
    }

    private void PlayCountPulseAndShake()
    {
        if (!countTM || countTMRT == null) return;

        countColorTween?.Kill();
        countShakeTween?.Kill();

        countTM.color = addGreenBase;
        countColorTween = countTM.DOColor(Color.white, countNumberDur).SetEase(Ease.OutCubic);

        countTMRT.localScale = countBaseScale;
        countShakeTween = countTMRT.DOPunchScale(Vector3.one * 0.12f, 0.18f, 10, 0.8f);
    }



    // Final adjustment
    public Tween WaveFoldToY0(float moveDur = 0.20f, float fadeDur = 0.20f, Ease ease = Ease.OutCubic, System.Action onArrive = null)
    {
        // 이미 비활성이면 스킵
        if (!bActive)
        {
            onArrive?.Invoke();
            return null;
        }

        bActive = false;

        moveTween?.Kill();
        fadeTween?.Kill();
        KillAddTweens();

        var rt = (RectTransform)transform;
        float x = rt.anchoredPosition.x;

        Sequence seq = DOTween.Sequence();
        seq.Join(rt.DOAnchorPos(new Vector2(x, 0f), moveDur).SetEase(ease));
        seq.Join(abilityGroup.DOFade(0f, fadeDur).SetEase(Ease.OutCubic));

        seq.OnComplete(() =>
        {
            onArrive?.Invoke();
            gameObject.SetActive(false);
        });

        return seq;
    }


    public void ResetBaseCount()
    {
        baseCount = 0;
        baseDisplayed = 0;
        if (countTM) countTM.text = "0";
    }

    public void ResetAllCounts()
    {
        ResetBaseCount();
        addTotal = 0;
        addDisplayed = 0;

        // Add 텍스트 숨김
        if (addCountTM)
        {
            addCountTM.text = "";
            addCountTM.gameObject.SetActive(false);
            var c = addCountTM.color; c.a = 0f; addCountTM.color = c;
        }

        // add 위치/스케일 복구
        if (addTMRT)
        {
            addTMRT.anchoredPosition = addTMBasePos;
            addTMRT.localScale = addTMBaseScale;
        }

        KillAddTweens();
    }
}
