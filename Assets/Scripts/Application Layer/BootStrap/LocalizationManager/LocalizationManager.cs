using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct CardTextData
{
    public int id;
    public string name;
    public string upgradedName;
    public string description;
    public string upgradedDescription;
}

[System.Serializable]
public struct LangDataWrapper
{
    public List<CardTextData> cards;
}

public class LocalizationManager : ICardLocalizationSystem
{
    private Dictionary<int, byte[]> nameMap = new Dictionary<int, byte[]>(100);
    private Dictionary<int, byte[]> upgradedNameMap = new Dictionary<int, byte[]>(100);
    private Dictionary<int, byte[]> descMap = new Dictionary<int, byte[]>(100);
    private Dictionary<int, byte[]> upgradedDescMap = new Dictionary<int, byte[]>(100);

    private StringBuilder sharedBuffer = new StringBuilder(500);

    private char[] charBuffer = new char[1024];
    public void LoadLanguage(string fileName)
    {
        nameMap.Clear();
        upgradedNameMap.Clear();
        descMap.Clear();
        upgradedDescMap.Clear();

        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset == null)
        {
            Debug.Log("Korean Localization Asset is null!");
            return;
        }

        LangDataWrapper wrapper = JsonUtility.FromJson<LangDataWrapper>(textAsset.text);

        foreach (var item in wrapper.cards)
        {
            if (!nameMap.ContainsKey(item.id))
            {
                nameMap.Add(item.id, Encoding.UTF8.GetBytes(item.name));
                upgradedNameMap.Add(item.id, Encoding.UTF8.GetBytes(item.upgradedName));
                descMap.Add(item.id, Encoding.UTF8.GetBytes(item.description));
                upgradedDescMap.Add(item.id, Encoding.UTF8.GetBytes(item.upgradedDescription));
            }
        }

        Resources.UnloadAsset(textAsset);
    }

    public void SetCardUIText(int id, TextMeshProUGUI targetName, TextMeshProUGUI targetUpgradedName, TextMeshProUGUI targetDesc, TextMeshProUGUI targetUpgradedDesc)
    {
        if (nameMap.TryGetValue(id, out byte[] nameBytes) && targetName)
        {
            BytesToBuffer(nameBytes); // 버퍼에 씀 (Alloc 0)
            targetName.SetText(sharedBuffer); // TMP가 SB를 읽음 (Alloc 0)
        }

        if (upgradedNameMap.TryGetValue(id, out byte[] upgradedNameBytes) && targetUpgradedName)
        {
            BytesToBuffer(upgradedNameBytes); // 버퍼에 씀 (Alloc 0)
            targetUpgradedName.SetText(sharedBuffer); // TMP가 SB를 읽음 (Alloc 0)
        }

        if (descMap.TryGetValue(id, out byte[] descBytes) && targetDesc)
        {
            BytesToBuffer(descBytes);
            targetDesc.SetText(sharedBuffer);
        }

        if (upgradedDescMap.TryGetValue(id, out byte[] upgradedDescBytes) && targetUpgradedDesc)
        {
            BytesToBuffer(upgradedDescBytes);
            targetUpgradedDesc.SetText(sharedBuffer);
        }
    }

    // 바이트 배열을 StringBuilder에 밀어넣는 함수
    private void BytesToBuffer(byte[] sourceBytes)
    {
        sharedBuffer.Clear();

        int charCount = Encoding.UTF8.GetCharCount(sourceBytes);

        if (charCount > charBuffer.Length)
        {
            charBuffer = new char[charCount + 256];
        }

        Encoding.UTF8.GetChars(sourceBytes, 0, sourceBytes.Length, charBuffer, 0);

        sharedBuffer.Append(charBuffer, 0, charCount);
    }
}