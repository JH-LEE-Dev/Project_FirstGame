using TMPro;
using UnityEngine;

public interface ICardLocalizationSystem
{
    void SetCardUIText(int id, TextMeshProUGUI targetName, TextMeshProUGUI targetUpgradedName, TextMeshProUGUI targetDesc, TextMeshProUGUI targetUpgradedDesc);
}
