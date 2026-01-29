using TMPro;
using UnityEngine;

public interface ICardLocalizationSystem
{
    void SetCardUIText(int id, TextMeshProUGUI targetName, TextMeshProUGUI targetDesc, TextMeshProUGUI targetUpgradedDesc);
}
