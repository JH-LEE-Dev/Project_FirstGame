using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionUI_Unit : MonoBehaviour
{
    private Image imageComp;
    private TMP_Text remainText;

    private int remainCnt = 0;

    private void Awake()
    {
        if (null == imageComp)
            imageComp = GetComponentInChildren<Image>();

        if (null == remainText)
            remainText = GetComponentInChildren<TMP_Text>();
    }

    private void OnDisable()
    {
        imageComp.sprite = null;
        remainText.text = null;
    }

    public void UpdateUnit(Sprite _image, int _remainCnt)
    {
        remainCnt = _remainCnt;

        UpdateImage(_image);
        UpdateRemainText(_remainCnt);
    }

    private void UpdateImage(Sprite _image)
    {
        if (null == _image || null == imageComp)
            return;

        imageComp.sprite = _image;
    }

    private void UpdateRemainText(int _remainCnt)
    {
        if (null == remainText)
            return;

        remainText.text = _remainCnt.ToString();
    }
}
