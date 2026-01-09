using DG.Tweening;
using UnityEngine;

public class DrawEffect : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Transform visual = null;
    [SerializeField] private float rotateDuration = 1f;

    private DeckSystem deckSystem = null;

    private Sequence activeSeq = null;
    private Tween activeRotate = null;

    private CardDataInstance cardDataInstance = null;
    public CardDataInstance CardDataInstance { set { cardDataInstance = value; } }

    public void Init(DeckSystem _deckSystem)
    {
        deckSystem = _deckSystem; 
    }

    public void PlayingDrawEvent(int _idx, float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        if (null != activeSeq && activeSeq.IsActive())
            activeSeq.Kill();

        LoopRotate();

        activeSeq = DOTween.Sequence();

        activeSeq.AppendInterval(_idx * _spawnDelay);

        activeSeq.Append(transform.DOPath(points, _drawDuration, PathType.CubicBezier, PathMode.TopDown2D, 10, Color.green)
            .SetUpdate(false)
            .SetEase(_drawEase)
            .OnComplete(() =>
            {
                deckSystem?.CallOneCardDrawCompleted(_idx, transform.position, cardDataInstance, gameObject);
                activeRotate.Kill();
            }));
    }

    private void LoopRotate()
    {
        if (null != activeRotate && activeRotate.IsActive())
            activeRotate.Kill();

        activeRotate = visual.DORotate(new Vector3(0f, 0f, 360f), rotateDuration, RotateMode.FastBeyond360)
            .SetUpdate(false)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}
 