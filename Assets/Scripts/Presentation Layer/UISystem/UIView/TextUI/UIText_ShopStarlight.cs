using TMPro;
using UnityEngine;

public class UIText_ShopStarlight : MonoBehaviour
{
    private TMP_Text mainText;

    private void Awake()
    {
        if (null == mainText)
            mainText = GetComponent<TMP_Text>();
    }

    public void UpdateText(int _current)
    {
        if (null == mainText)
            return;

        mainText.text = _current.ToString();
    }
}
