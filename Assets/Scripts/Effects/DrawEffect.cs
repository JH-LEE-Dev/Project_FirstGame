using DG.Tweening;
using UnityEngine;

public class DrawEffect : MonoBehaviour
{
    private DeckSystem deckSystem = null;
    private Sequence activeSeq = null;

    private CardDataInstance cardDataInstance = null;
    public CardDataInstance CardDataInstance { set { cardDataInstance = value; } }

    public void Init(DeckSystem _deckSystem)
    {
        deckSystem = _deckSystem; 
    }

    public void PlayingDrawEvent(float _spawnDelay, float _drawDuration, Ease _drawEase, Vector3[] points)
    {
        Debug.Log("카드 드로우");

        if (null != activeSeq && activeSeq.IsActive())
            activeSeq.Kill();

        activeSeq = DOTween.Sequence();

        activeSeq.AppendInterval(_spawnDelay);

        activeSeq.Append(transform.DOPath(points, _drawDuration, PathType.CatmullRom, PathMode.TopDown2D)
            .SetUpdate(false)
            .SetEase(_drawEase)
            .OnComplete(() =>
            {
                deckSystem?.CallOneCardDrawCompleted(transform.position, cardDataInstance, gameObject);
                Debug.Log("한장 끝");
            }));
    }
}
