using DG.Tweening;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CutsceneComponent : MonoBehaviour
{
    private Character character;
    private ICutsceneSignalHandler cutsceneSignalHandler;

    [Header("Turn Start Points")]
    [SerializeField] private Transform turnStartPointPrefab;

    private Transform turnStartPoint;
    private Sequence turnStartSeq;


    // 이 변수는, 무조건 "연출 중"인 순간에만 true로 된다.
    private bool bCutscene = false;
    public bool IsCutscene => bCutscene;

    [Header("TurnStart")]
    [SerializeField] private float turnStartDur = 0.8f;
    [SerializeField] private float maxTurnStartScale = 12f;
    private float OriginScale = 4.25f;


    public void Initialize(Character _character, ICutsceneSignalHandler _handler)
    {
        character = _character;
        cutsceneSignalHandler = _handler;
        turnStartPoint = Instantiate(turnStartPointPrefab, null);
    }

    [ContextMenu("TEST / Turn Start Cutscene")]
    public void TurnStart()
    {
        if (bCutscene) return;

        if (character == null || turnStartPoint == null)
        {
            Debug.LogWarning("[CutsceneComponent] TurnStart failed: missing refs.");
            return;
        }

        bCutscene = true;

        cutsceneSignalHandler?.NotifyCutsceneSignalAction(CutsceneSignal.TurnStart_Start);

        turnStartSeq?.Kill();
        turnStartSeq = null;

        Transform ct = character.transform;

        Vector3 startPos = ct.position;
        Vector3 endPos = turnStartPoint.position;

        Vector3 startScale = ct.localScale;
        Vector3 targetScale = Vector3.one * maxTurnStartScale;

        turnStartSeq = DOTween.Sequence();

        Tween moveTween = ct.DOMove(endPos, turnStartDur)
            .SetEase(Ease.OutCubic);

        Tween scaleTween = ct.DOScale(targetScale, turnStartDur)
            .From(startScale, true)
            .SetEase(Ease.OutCubic);

        turnStartSeq
            .Join(moveTween)
            .Join(scaleTween)
            .OnComplete(() =>
            {
                bCutscene = false;
                cutsceneSignalHandler?.NotifyCutsceneSignalAction(CutsceneSignal.TurnStart_End);
            });
    }
    [ContextMenu("TEST / Turn End Cutscene")]
    public void TurnEnd()
    {
        if (bCutscene) return;

        if (character == null)
        {
            Debug.LogWarning("[CutsceneComponent] TurnEnd failed: character is null.");
            return;
        }

        PMoveComponent mc = character.GetComponent<PMoveComponent>();
        if (mc == null)
        {
            Debug.LogWarning("[CutsceneComponent] TurnEnd failed: PMoveComponent not found.");
            return;
        }

        Vector3 crp = mc.GetCharacterResetPosition();

        bCutscene = true;

        // 컷씬 시작 신호 (즉시)
        cutsceneSignalHandler?.NotifyCutsceneSignalAction(CutsceneSignal.TurnEnd_Start);

        // 기존 트윈 정리
        turnStartSeq?.Kill();
        turnStartSeq = null;

        Transform ct = character.transform;

        Vector3 startPos = ct.position;
        Vector3 endPos = crp;

        Vector3 startScale = ct.localScale;
        Vector3 targetScale = Vector3.one * OriginScale;


        bool endCalled = false;
        void FinishTurnEnd()
        {
            if (endCalled) return;
            endCalled = true;
            cutsceneSignalHandler?.NotifyCutsceneSignalAction(CutsceneSignal.TurnEnd_End);
            bCutscene = false;
        }
        turnStartSeq = DOTween.Sequence();

        Tween moveTween = ct.DOMove(endPos, turnStartDur)
            .SetEase(Ease.OutCubic);

        Tween scaleTween = ct.DOScale(targetScale, turnStartDur)
            .From(startScale, true)
            .SetEase(Ease.OutCubic);

        turnStartSeq
            .Join(moveTween)
            .Join(scaleTween)
            .OnComplete(FinishTurnEnd);
    }

    public void GameOver()
    {
        bCutscene = true;



    }

    public void WaveClear()
    {
        bCutscene = true;



    }
}
