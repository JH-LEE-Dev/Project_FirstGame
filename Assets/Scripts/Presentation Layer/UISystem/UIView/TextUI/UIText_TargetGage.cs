using TMPro;
using UnityEngine;

public class UIText_TargetGage : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TMP_Text mainText;
    
    public void DataUpdate(int currentKillCnt, int maxCnt)
    {
        if (null == mainText)
            return;

        string currText = Mathf.RoundToInt(currentKillCnt).ToString();
        string maxText = Mathf.RoundToInt(maxCnt).ToString();

        mainText.text = (currText + "\\" + maxText);
    }
}
