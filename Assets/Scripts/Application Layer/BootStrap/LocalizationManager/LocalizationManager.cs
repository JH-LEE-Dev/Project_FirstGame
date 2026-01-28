using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 1. 텍스트 데이터를 담을 그릇 (가벼운 클래스)
[System.Serializable]
public class CardTextData
{
    public int id;
    public string name; // Name
    public string description; // Description
}

// 2. JSON 파싱용 래퍼
[System.Serializable]
public class LangDataWrapper
{
    public List<CardTextData> data;
}

public class LocalizationManager
{
    private Dictionary<int, CardTextData> textMap = new Dictionary<int, CardTextData>(50);

    public void LoadLanguage(string fileName)
    {
        textMap.Clear();
        string path = Path.Combine(Application.streamingAssetsPath, fileName + ".json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LangDataWrapper wrapper = JsonUtility.FromJson<LangDataWrapper>(json);

            // 리스트를 딕셔너리로 고속 변환
            foreach (var item in wrapper.data)
            {
                // 중복 ID 방지 체크 후 삽입
                if (!textMap.ContainsKey(item.id))
                {
                    textMap.Add(item.id, item);
                }
            }
        }
    }

    // [사용 함수] ID만 던지면 텍스트 뭉치를 줌
    public CardTextData GetCardText(int id)
    {
        if (textMap.TryGetValue(id, out CardTextData data))
        {
            return data;
        }
        return null; // 혹은 "Missing Text" 더미 데이터 반환
    }
}