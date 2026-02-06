using TMPro;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    [Header("Main Settings")]

    private WarningMotion motion;
    private TMP_Text mainText;

    private void Awake()
    {
        motion = GetComponent<WarningMotion>();
        mainText = GetComponentInChildren<TMP_Text>();
    }

    public void Play(string str)
    {
        gameObject.SetActive(true);
        SetText(str);
        motion?.PlayMotion(CompletedCallback);
    }

    public void Allkill() => motion?.AllKill();

    private void CompletedCallback()
    {
        gameObject.SetActive(false);
    }

    private void SetText(string str)
    {
        if (null == mainText)
            return;

        mainText.text = str;
    }
}
