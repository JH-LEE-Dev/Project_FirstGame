using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CountUIType
{
    HideWhenZero,           // 0일땐, 보이지 않게 하고, 1 이상일때만 보이게 함.
    VisibleWhenZero         // 0일때도 보이게 한다.
}

public class CountUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI countTM;
    [SerializeField] private Image countBG;

    // 표시하고 싶은 카운트
    private int count = 0;

    // 이 UI의 타입 설정
    private CountUIType countUIType;



    // 타입, 초기 카운트 세팅 할거면 할 것.
    // 끝에 카운트 안 넣으면 현 count유지하고 타입만 변경됨.
    public void TypeSetting(CountUIType _type, int _count = -1)
    {
        // 매개 변수를 넣었을 때만 count값이 바뀐다.
        if (_count != -1) count = _count;

        countUIType = _type;
        SetCount(count);
    }

    // 숫자를 변경해준다. (CountUIType.HideWhenZero으로 했으면 0일땐 아예 UI가 가려짐)
    public void SetCount(int _count)
    {
        if (countTM == null) return;

        if (_count <= 0 && countUIType == CountUIType.HideWhenZero)
        {
            count = 0;
            SetActiveCountUI(false);
            return;
        }

        count = _count;
        SetActiveCountUI(true);
        countTM.SetText(count.ToString());
    }

    // 현재 숫자 반환 함수
    public int GetCount()
    {
        return count;
    }



    // private
    private void SetActiveCountUI(bool _isActive)
    {
        countTM.gameObject.SetActive(_isActive);
        countBG.gameObject.SetActive(_isActive);
    }
}
