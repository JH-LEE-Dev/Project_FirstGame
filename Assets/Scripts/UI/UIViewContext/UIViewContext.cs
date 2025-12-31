using UnityEngine;

public class UIViewContext
{
    public IDeckProvider deckProvider;

    public void Initialize(IDeckProvider _deckProvider)
    {
        deckProvider = _deckProvider;
    }

    public void ResetVariable()
    {
        deckProvider = null;
    }
}
