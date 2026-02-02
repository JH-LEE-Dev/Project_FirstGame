using DG.Tweening;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CutsceneComponent : MonoBehaviour
{
    private Character character;
    private Character_Visual characterVisual;
    private ICutsceneSignalHandler cutsceneSignalHandler;
    private IOrbitPathProvider orbitPathProvider;

    [Header("Turn Start Points")]
    [SerializeField] private Transform turnStartPointPrefab;

    private Transform turnStartPoint;
    private Sequence turnStartSeq;


    // 이 변수는, 무조건 "연출 중"인 순간에만 true로 된다.
    private bool bCutscene = false;
    public bool IsCutscene => bCutscene;

    [Header("TurnStart")]
    [SerializeField] private float turnStartDur = 1f;
    [SerializeField] private float turnEndDur = 0.5f;
    [SerializeField] private float maxTurnStartScale = 12f;
    private float OriginScale = 4.25f;


    public void Initialize(Character _character, ICutsceneSignalHandler _handler, IOrbitPathProvider _orbitPathProvider, Character_Visual _characterVisual)
    {
        character = _character;
        cutsceneSignalHandler = _handler;
        orbitPathProvider = _orbitPathProvider;
        characterVisual = _characterVisual;
        turnStartPoint = Instantiate(turnStartPointPrefab, null);
    }

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
        orbitPathProvider.SetPathActive(false);

        turnStartSeq?.Kill();
        turnStartSeq = null;

        Transform ct = character.transform;

        Vector3 startPos = ct.position;
        Vector3 endPos = turnStartPoint.position;

        Vector3 startScale = ct.localScale;
        Vector3 targetScale = Vector3.one * maxTurnStartScale;

        turnStartSeq = DOTween.Sequence();

        Tween moveTween = null;
        moveTween = ct.DOMove(endPos, turnStartDur)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                if (characterVisual == null || moveTween == null) return;

                float t = moveTween.ElapsedPercentage();
                float speedFeel = 1f - t;                
                speedFeel *= speedFeel;                  

                // Right로 기울이기: 네 규칙상 Right는 음수
                float ringLean = -28f * speedFeel;
                float bodyLean = -10f * speedFeel;

                characterVisual.SetCutsceneLeanTargets(ringLean, bodyLean);
            });

        Tween scaleTween = ct.DOScale(targetScale, turnStartDur)
            .From(startScale, true)
            .SetEase(Ease.OutCubic);

        turnStartSeq
            .Join(moveTween)
            .Join(scaleTween)
            .OnComplete(() =>
            {
                bCutscene = false;
                characterVisual?.ClearCutsceneLeanTargets();
                cutsceneSignalHandler?.NotifyCutsceneSignalAction(CutsceneSignal.TurnStart_End);
            });
    }
    public void TurnEnd()
    {
        //if (bCutscene) return;

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
        orbitPathProvider.SetPathActive(true);

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
            characterVisual?.ClearCutsceneLeanTargets();
            bCutscene = false;
        }
        turnStartSeq = DOTween.Sequence();



        Tween moveTween = null;
        moveTween = ct.DOMove(endPos, turnEndDur)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                if (characterVisual == null || moveTween == null) return;

                float t = moveTween.ElapsedPercentage();
                float speedFeel = 1f - t;
                speedFeel *= speedFeel;

                // Right로 기울이기: 네 규칙상 Right는 음수
                float ringLean = -28f * speedFeel;
                float bodyLean = -10f * speedFeel;

                characterVisual.SetCutsceneLeanTargets(ringLean, bodyLean);
            });

        Tween scaleTween = ct.DOScale(targetScale, turnEndDur)
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
