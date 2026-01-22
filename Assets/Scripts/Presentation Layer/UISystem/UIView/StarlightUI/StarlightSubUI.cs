using DG.Tweening;
using TMPro;
using UnityEngine;


public class StarlightSubUI : MonoBehaviour
{
    private bool bActive = false;
    private Vector2 targetPos = Vector2.zero;

    [SerializeField] private TextMeshProUGUI countTM;
    [SerializeField] private CanvasGroup abilityGroup;

    [Header("Tween")]
    [SerializeField] private float appearOffset = 10f;  // 왼쪽에서 시작
    [SerializeField] private float appearDur = 0.18f;   // 첫 등장 슬라이드
    [SerializeField] private float moveDur = 0.20f;     // 재배치 이동
    [SerializeField] private Ease appearEase = Ease.OutCubic;
    [SerializeField] private Ease moveEase = Ease.OutCubic;

    private Tween moveTween;
    private Tween fadeTween;

    public void Init()
    {
        bActive = false;
        abilityGroup.alpha = 0;

        //gameObject.SetActive(false);
    }

    public void StartSubUIActive(Vector2 arcPos)
    {
        bActive = true;

        RectTransform rt = GetComponent<RectTransform>();

        targetPos = arcPos;

        Vector2 startPos = arcPos + Vector2.left * appearOffset;
        rt.localPosition = startPos;

        moveTween?.Kill();
        fadeTween?.Kill();

        abilityGroup.alpha = 0f;
        gameObject.SetActive(true);

        moveTween = rt.DOLocalMove(targetPos, appearDur).SetEase(appearEase);
        fadeTween = abilityGroup.DOFade(1f, appearDur).SetEase(appearEase);
    }

    public bool GetSubUIActive() => bActive;

    public void SetPosition(Vector2 arcPos)
    {
        // 활성 아니면 그냥 target만 업데이트(혹시 모를 호출 대비)
        targetPos = arcPos;
        if (!bActive) return;

        RectTransform rt = GetComponent<RectTransform>();

        // 이미 같은 위치면 스킵(잔떨림 방지)
        if (((Vector2)rt.localPosition - targetPos).sqrMagnitude < 0.01f * 0.01f)
            return;

        moveTween?.Kill();
        moveTween = rt.DOLocalMove(targetPos, moveDur).SetEase(moveEase);
    }

    public void ForceDeactivate()
    {
        bActive = false;
        moveTween?.Kill();
        fadeTween?.Kill();
        if (abilityGroup) abilityGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
