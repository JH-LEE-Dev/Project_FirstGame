using TMPro;
using UnityEngine;

public interface ICardLocalizationSystem
{
    void SetCardUIText(int id, TMP_Text targetName, TMP_Text targetDesc, TMP_Text targetUpgradedDesc);
}
